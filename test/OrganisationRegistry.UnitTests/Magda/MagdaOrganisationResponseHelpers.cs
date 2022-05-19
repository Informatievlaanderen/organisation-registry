namespace OrganisationRegistry.UnitTests.Magda
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Api.Infrastructure.Magda;
    using Autofac.Features.OwnedInstances;
    using Autofac.Util;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Magda;
    using SqlServer.Infrastructure;
    using Xunit;

    public class MagdaOrganisationResponseHelpers
    {
        private readonly ApiConfigurationSection _apiConfiguration;

        public MagdaOrganisationResponseHelpers()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                .AddEnvironmentVariables();

            var configurationRoot = configurationBuilder.Build();
            _apiConfiguration = configurationRoot.GetSection(ApiConfigurationSection.Name).Get<ApiConfigurationSection>();
        }

        private GeefOndernemingQuery CreateGeefOndernemingQuery()
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
                capacity: null,
                recipient: _apiConfiguration.KboRecipient,
                kboMagdaEndPoint: _apiConfiguration.KboMagdaEndpoint,
                repertoriumMagdaEndpoint: _apiConfiguration.RepertoriumMagdaEndpoint,
                repertoriumCapacity: _apiConfiguration.RepertoriumCapacity);
            return new GeefOndernemingQuery(
                configuration: magdaConfiguration,
                contextFactory: () => new Owned<OrganisationRegistryContext>(
                    new OrganisationRegistryContext(
                        new DbContextOptionsBuilder<OrganisationRegistryContext>()
                            .UseInMemoryDatabase(
                                "org-magda-test",
                                builder => { })
                            .Options), new Disposable()),
                httpClientFactory: httpClientFactoryMock.Object,
                new NullLogger<GeefOndernemingQuery>());
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

            var envelope = await CreateGeefOndernemingQuery()
                .Execute(
                    Mock.Of<IUser>(),
                    kboNumber);

            envelope.Should().NotBeNull();

            var json = JsonConvert.SerializeObject(envelope, Formatting.Indented);

            await File.WriteAllTextAsync(Path.Join(magdaResponsesDir, $"{kboNumber}.json"), json);
        }
    }
}
