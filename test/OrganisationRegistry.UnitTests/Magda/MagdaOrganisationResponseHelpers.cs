namespace OrganisationRegistry.UnitTests.Magda
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Api.Configuration;
    using Api.Kbo;
    using Autofac.Features.OwnedInstances;
    using Autofac.Util;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using OrganisationRegistry.Magda;
    using OrganisationRegistry.Magda.Common;
    using OrganisationRegistry.Magda.Responses;
    using SqlServer.Infrastructure;
    using Xunit;

    public class MagdaOrganisationResponseHelpers
    {
        private readonly ApiConfiguration _apiConfiguration;

        public MagdaOrganisationResponseHelpers()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();

            var configurationRoot = configurationBuilder.Build();
            _apiConfiguration = configurationRoot.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>();
        }

        private GeefOndernemingQuery CreateGeefOndernemingQuery()
        {
            return new GeefOndernemingQuery(
                configuration: new MagdaConfiguration(
                    clientCertificate: new X509Certificate2(
                        rawData: Convert.FromBase64String(_apiConfiguration.KboCertificate),
                        password: string.Empty,
                        keyStorageFlags: X509KeyStorageFlags.MachineKeySet |
                                         X509KeyStorageFlags.PersistKeySet |
                                         X509KeyStorageFlags.Exportable),
                    timeout: _apiConfiguration.KboMagdaTimeout,
                    sender: _apiConfiguration.KboSender,
                    capacity: null,
                    recipient: _apiConfiguration.KboRecipient,
                    kboMagdaEndPoint: _apiConfiguration.KboMagdaEndpoint),
                contextFactory: () => new Owned<OrganisationRegistryContext>(
                    new OrganisationRegistryContext(
                        new DbContextOptionsBuilder<OrganisationRegistryContext>()
                            .UseInMemoryDatabase(
                                "org-magda-test",
                                builder => { })
                            .Options), new Disposable()));
        }

        // [Theory]
        [Theory(Skip = "Run manually, integrates with magda " +
                       "and writes files locally to MagdaResponses folder " +
                       "to run tests on.")]
        [InlineData("0542399749")]
        [InlineData("0863557445")]
        [InlineData("0404055577")]
        [InlineData("0845851975")]
        [InlineData("0859047440")]
        [InlineData("0860325266")]
        [InlineData("0449091588")]
        public async Task SerializeMagdaResponse(string kboNumber)
        {
            var magdaResponsesDir =
                Path.Join(
                    Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.FullName,
                    "MagdaResponses");

            var envelope = await CreateGeefOndernemingQuery()
                .Execute(
                    new GenericPrincipal(new GenericIdentity("magda test"), new string[0]),
                    kboNumber);

            envelope.Should().NotBeNull();

            var json = JsonConvert.SerializeObject(envelope, Formatting.Indented);

            await File.WriteAllTextAsync(Path.Join(magdaResponsesDir, $"{kboNumber}.json"), json);
        }
    }
}
