namespace OrganisationRegistry.Api.Infrastructure;

using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersionNeutral]
[Route("")]
public class EmptyController : OrganisationRegistryController
{
    public EmptyController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromServices] IConfiguration configuration)
    {
        var version = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
        return await ContentAsync($"Welcome to the OrganisationRegistry Api v{version}. You can find the OrganisationRegistry website at {configuration["UI:Endpoint"]}");
    }
}
