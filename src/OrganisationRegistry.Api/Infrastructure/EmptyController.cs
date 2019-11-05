namespace OrganisationRegistry.Api.Infrastructure
{
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [ApiVersionNeutral]
    [Route("")]
    public class EmptyController : Controller
    {
        [HttpGet]
        public IActionResult Get([FromServices] IConfiguration configuration)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            return Content($"Welcome to the OrganisationRegistry Api v{version}. You can find the OrganisationRegistry website at {configuration["UI:Endpoint"]}");
        }
    }
}
