namespace OrganisationRegistry.Api.Backoffice.Admin.Status
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using ElasticSearch.Configuration;
    using Infrastructure;
    using Infrastructure.Helpers;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Security;
    using SqlServer.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("status")]
    public class StatusController : OrganisationRegistryController
    {
        public StatusController(ICommandSender commandSender) : base(commandSender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("I'm ok!");
        }

        [HttpGet]
        [Route("toggles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToggles([FromServices] IOptions<TogglesConfigurationSection> toggles)
        {
            return Ok(toggles.Value);
        }

        [HttpGet]
        [Route("features")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeatures([FromServices] IFeatureManager featureManager)
        {
            var features = new Dictionary<string, bool>();
            await foreach (var featureName in featureManager.GetFeatureNamesAsync())
            {
                features.Add(featureName.ToCamelCase(), await featureManager.IsEnabledAsync(featureName));
            }
            return Ok(features);
        }

        [HttpGet]
        [Route("configuration")]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        public async Task<IActionResult> GetConfiguration(
            [FromServices] IConfiguration configuration,
            [FromServices] IExternalIpFetcher externalIpFetcher)
        {
            var apiConfiguration = configuration.GetSection(ApiConfigurationSection.Name).Get<ApiConfigurationSection>();

            var summary = new
            {
                Api = apiConfiguration,
                Configuration = configuration.GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>().Obfuscate(),
                ElasticSearch = configuration.GetSection(ElasticSearchConfiguration.Section).Get<ElasticSearchConfiguration>().Obfuscate(),
                Infrastructure = configuration.GetSection(InfrastructureConfigurationSection.Name).Get<InfrastructureConfigurationSection>().Obfuscate(),
                Logging = PrintConfig(configuration.GetSection("Logging")),
                Serilog = PrintConfig(configuration.GetSection("Serilog")),
                SqlServer = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>().Obfuscate(),
                Toggles = configuration.GetSection(TogglesConfigurationSection.Name).Get<TogglesConfigurationSection>(),
                Ip = await externalIpFetcher.Fetch()
            };

            var jsonSerializerSettings = JsonConvert.DefaultSettings();
            var resolver = (DefaultContractResolver)jsonSerializerSettings.ContractResolver;
            resolver.NamingStrategy.ProcessDictionaryKeys = true;

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int) HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(summary, Formatting.Indented, jsonSerializerSettings)
            };
        }

        private static Dictionary<string, object> PrintConfig(IConfiguration configuration)
        {
            return configuration.GetChildren().ToDictionary(x => x.Key, x => (object)x.Value ?? PrintConfig(x));
        }
    }
}