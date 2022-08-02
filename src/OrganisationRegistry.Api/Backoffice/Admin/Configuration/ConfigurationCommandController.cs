namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration;

using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Configuration.Database;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("configuration")]
[OrganisationRegistryAuthorize(Role.Developer)]
public class ConfigurationCommandController : OrganisationRegistryController
{
    public ConfigurationCommandController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Create a configuration value.</summary>
    /// <response code="201">If the configuration value is created, together with the location.</response>
    /// <response code="400">If the configuration value does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] ConfigurationContext context, [FromBody] CreateConfigurationValueRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        context.Configuration.Add(new ConfigurationValue(message.Key, message.Description, message.Value));
        await context.SaveChangesAsync();

        return CreatedWithLocation(nameof(ConfigurationController),nameof(ConfigurationController.Get), new { id = message.Key });
    }

    /// <summary>Update a configuration value.</summary>
    /// <response code="200">If the configuration value is updated, together with the location.</response>
    /// <response code="400">If the configuration value does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromServices] ConfigurationContext context, [FromRoute] string id, [FromBody] UpdateConfigurationValueRequest message)
    {
        var internalMessage = new UpdateConfigurationValueInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        var configurationValue = context.Configuration.Single(x => x.Key == id);
        configurationValue.Value = internalMessage.Body.Value;
        configurationValue.Description = internalMessage.Body.Description;
        await context.SaveChangesAsync();

        return OkWithLocationHeader(nameof(ConfigurationController),nameof(ConfigurationController.Get), new { id = internalMessage.Key });
    }
}
