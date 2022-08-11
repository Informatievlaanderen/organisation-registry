namespace OrganisationRegistry.UnitTests.Magda;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using OrganisationRegistry.Api.Infrastructure.Magda;
using Autofac.Features.OwnedInstances;
using Autofac.Util;
using FluentAssertions;
using global::Magda.RegistreerInschrijving;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Magda;
using SqlServer.Infrastructure;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class MagdaOrganisationResponseHelpers
{
    private readonly ITestOutputHelper _logger;

    private record MagdaSetup(MagdaConfiguration MagdaConfiguration, IHttpClientFactory HttpClientFactory, Func<Owned<OrganisationRegistryContext>> ContextFactory);

    private readonly ApiConfigurationSection _apiConfiguration;

    public MagdaOrganisationResponseHelpers(ITestOutputHelper logger)
    {
        _logger = logger;
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .AddEnvironmentVariables();

        var configurationRoot = configurationBuilder.Build();
        _apiConfiguration = configurationRoot.GetSection(ApiConfigurationSection.Name).Get<ApiConfigurationSection>();
    }

    private MagdaSetup CreateMagdaSetup()
    {
        var magdaClientCertificate = MagdaClientCertificate.Create(
            _apiConfiguration.KboCertificate,
            string.Empty);

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(factory => factory.CreateClient(MagdaModule.HttpClientName))
            .Returns(() => new HttpClient(new MagdaHttpClientHandler(magdaClientCertificate)));

        var magdaConfiguration = new MagdaConfiguration(
            clientCertificate: magdaClientCertificate,
            timeout: _apiConfiguration.KboMagdaTimeout,
            sender: _apiConfiguration.KboSender,
            capacity: string.Empty,
            recipient: _apiConfiguration.KboRecipient,
            kboMagdaEndPoint: _apiConfiguration.KboMagdaEndpoint,
            repertoriumMagdaEndpoint: _apiConfiguration.RepertoriumMagdaEndpoint,
            repertoriumCapacity: _apiConfiguration.RepertoriumCapacity);

        var contextFactory = () => new Owned<OrganisationRegistryContext>(
            new OrganisationRegistryContext(
                new DbContextOptionsBuilder<OrganisationRegistryContext>()
                    .UseInMemoryDatabase(
                        "org-magda-test",
                        _ => { })
                    .Options), new Disposable());

        return new MagdaSetup(magdaConfiguration, httpClientFactoryMock.Object, contextFactory);
    }

    // [Theory]
    [Theory(Skip = "Run manually, integrates with magda " +
                   "and writes files locally to MagdaResponses folder " +
                   "to run tests on.")]
    [InlineData("0542399749")]
    [InlineData("0863557445")]
    [InlineData("0404055577")]
    [InlineData("0845851975")]
    // [InlineData("0859047440")] // if enabled, make sure to scrub personal data
    [InlineData("0860325266")]
    [InlineData("0449091588")]
    [InlineData("0472225692")]
    [InlineData("0471693974")]
    [InlineData("0202239951")]
    [InlineData("0400523292")]
    [InlineData("0404221962")]
    [InlineData("0404221000")]

    public async Task SerializeMagdaResponse(string kboNumber)
    {
        var magdaResponsesDir =
            Path.Join(
                Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.Parent!.Parent!.Parent!.FullName,
                "MagdaResponses");

        var magdaSetup = CreateMagdaSetup();

        var maybeRegistration = await new RegistreerInschrijvingCommand(
                magdaSetup.ContextFactory,
                magdaSetup.MagdaConfiguration,
                magdaSetup.HttpClientFactory,
                new NullLogger<RegistreerInschrijvingCommand>())
            .Execute(TestUser.AlgemeenBeheerder, kboNumber);

        if (maybeRegistration is not {} registration)
            throw new Exception("Geen antwoord van magda gekregen.");

        var maybeReply = registration.Body?.RegistreerInschrijvingResponse?.Repliek?.Antwoorden?.Antwoord;
        if (maybeReply is not {} reply)
            throw new Exception("Geen antwoord van magda gekregen.");

        reply.Uitzonderingen?
            .Where(type => type.Type == UitzonderingTypeType.INFORMATIE)
            .ToList()
            .ForEach(type => _logger.WriteLine($"{type.Diagnose}"));

        reply.Uitzonderingen?
            .Where(type => type.Type == UitzonderingTypeType.WAARSCHUWING)
            .ToList()
            .ForEach(type => _logger.WriteLine($"{type.Diagnose}"));

        var errors = reply.Uitzonderingen?
            .Where(type => type.Type == UitzonderingTypeType.FOUT)
            .ToList() ?? new List<UitzonderingType>();

        errors.ForEach(type => _logger.WriteLine($"{type.Diagnose}"));

        if (errors.Any())
            throw new Exception(
                $"Fout in magda response:\n" +
                $"{string.Join('\n', errors.Select(type => type.Diagnose))}");

        var query = new GeefOndernemingQuery(
            magdaSetup.MagdaConfiguration,
            magdaSetup.ContextFactory,
            magdaSetup.HttpClientFactory,
            new NullLogger<GeefOndernemingQuery>());

        var envelope = await query
            .Execute(
                Mock.Of<IUser>(),
                kboNumber);

        envelope.Should().NotBeNull();

        var json = JsonConvert.SerializeObject(envelope, Formatting.Indented);

        await File.WriteAllTextAsync(Path.Join(magdaResponsesDir, $"{kboNumber}.json"), json);
    }
}
