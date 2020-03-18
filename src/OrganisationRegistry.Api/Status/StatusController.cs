namespace OrganisationRegistry.Api.Status
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Configuration;
    using ElasticSearch.Configuration;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Security;
    using SqlServer.Configuration;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("status")]
    public class StatusController : OrganisationRegistryController
    {
        public StatusController(ICommandSender commandSender) : base(commandSender)
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("I'm ok!");
        }

        [HttpGet]
        [Route("toggles")]
        //[OrganisationRegistryAuthorize(Roles = Roles.Developer)] // don't authorize here, we use this one to determine UI functionalities.
        [ProducesResponseType(typeof(TogglesConfiguration), (int)HttpStatusCode.OK)]
        public IActionResult GetToggles([FromServices] IOptions<TogglesConfiguration> toggles)
        {
            return Ok(toggles.Value);
        }

        [HttpGet]
        [Route("configuration")]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        public async System.Threading.Tasks.Task<IActionResult> GetConfiguration(
            [FromServices] IConfiguration configuration,
            [FromServices] IHttpClientFactory httpClientFactory)
        {
            var apiConfiguration = configuration.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>();

            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(apiConfiguration.ExternalIpServiceUri);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpResponseMessage = await httpClient.GetAsync("");
            var summary = new
            {
                Api = apiConfiguration,
                Configuration = configuration.GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>().Obfuscate(),
                ElasticSearch = configuration.GetSection(ElasticSearchConfiguration.Section).Get<ElasticSearchConfiguration>().Obfuscate(),
                Infrastructure = configuration.GetSection(InfrastructureConfiguration.Section).Get<InfrastructureConfiguration>().Obfuscate(),
                Logging = PrintConfig(configuration.GetSection("Logging")),
                Serilog = PrintConfig(configuration.GetSection("Serilog")),
                SqlServer = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>().Obfuscate(),
                Toggles = configuration.GetSection(TogglesConfiguration.Section).Get<TogglesConfiguration>(),
                Ip = await httpResponseMessage.Content.ReadAsStringAsync()
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
