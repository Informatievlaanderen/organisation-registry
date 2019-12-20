namespace OrganisationRegistry.Api.Dump
{
    using AgentschapZorgEnGezondheid;
    using ElasticSearch.Client;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using System.Xml;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
    using Newtonsoft.Json.Serialization;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("dumps")]
    [FormatFilter]
    public class DumpController : OrganisationRegistryController
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        public DumpController(
            ICommandSender commandSender,
            IActionContextAccessor actionContextAccessor)
            : base(commandSender)
        {
            _actionContextAccessor = actionContextAccessor;
        }

        /// <summary>
        /// Get all organisations.
        /// </summary>
        /// <param name="elastic"></param>
        /// <returns></returns>
        [HttpGet("agentschap-zorg-en-gezondheid/full")]
        [HttpGet("agentschap-zorg-en-gezondheid/full.{format}")]
        [ProducesResponseType(typeof(IEnumerable<OrganisationDump>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrganisations([FromServices] Elastic elastic)
        {
            var format = _actionContextAccessor.ActionContext.GetValueFromHeader("format")
                ?? _actionContextAccessor.ActionContext.GetValueFromRouteData("format")
                ?? _actionContextAccessor.ActionContext.GetValueFromQueryString("format");

            var dump = await OrganisationDump.Search(
                elastic.ReadClient,
                ScrollSize,
                ScrollTimeout);

            var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

            var resolver = (OrganisationRegistryContractResolver)jsonSerializerSettings.ContractResolver;
            resolver.SetStringDefaultValueToEmptyString = true;

            string result, contentType;
            if (format == "xml")
            {
                resolver.NamingStrategy = new PascalCaseNamingStrategy();
                var json = JsonConvert.SerializeObject(dump, jsonSerializerSettings);

                var xml = JsonConvert.DeserializeXmlNode($"{{\"Organisation\":{json}}}", "Organisations");
                XmlNode docNode = xml.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                xml.InsertBefore(docNode, xml.DocumentElement);

                result = xml.OuterXml.Replace("T00:00:00", string.Empty);
                contentType = "text/xml";
            }
            else
            {
                result = JsonConvert.SerializeObject(dump, jsonSerializerSettings);
                contentType = "application/json";
                format = "json";
            }

            // Response.Headers.Add("Content-Disposition", $"attachment; filename=\"full.{format}\"");
            return File(Encoding.UTF8.GetBytes(result), contentType, $"full.{format}");
        }
    }

    public class PascalCaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? name : name.Insert(0, name[0].ToString().ToUpper()).Remove(1, 1);
        }
    }

    public static class HttpRequestExtensions
    {
        public static string GetValueFromRouteData(this ActionContext context, string key)
        {
            if (!context.RouteData.Values.TryGetValue(key, out var value)) return null;

            var routeValue = value?.ToString();

            return string.IsNullOrEmpty(routeValue) ? null : routeValue;
        }

        public static string GetValueFromQueryString(this ActionContext context, string key)
        {
            return context.HttpContext.Request.Query.TryGetValue(key, out var queryValue) ? queryValue.ToString() : null;
        }

        public static string GetValueFromHeader(this ActionContext context, string key)
        {
            return context.HttpContext.Request.Headers.TryGetValue(key, out var headerValue) ? headerValue.ToString() : null;
        }
    }
}
