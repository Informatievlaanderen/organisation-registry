namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFrameworkCategory;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("formalframeworkcategories")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class FormalFrameworkCategoryCommandController : OrganisationRegistryCommandController
{
    public FormalFrameworkCategoryCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een toepassingsgebiedcategorie.</summary>
    /// <response code="201">Als de toepassingsgebiedcategorie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de toepassingsgebiedcategorie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateFormalFrameworkCategoryRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateFormalFrameworkCategoryRequestMapping.Map(message));

        return CreatedWithLocation(nameof(FormalFrameworkCategoryController), nameof(FormalFrameworkCategoryController.Get), new { id = message.Id });
    }

    /// <summary>Pas een toepassingsgebiedcategorie aan.</summary>
    /// <response code="200">Als de toepassingsgebiedcategorie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de toepassingsgebiedcategorie mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkCategoryRequest message)
    {
        var internalMessage = new UpdateFormalFrameworkCategoryInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateFormalFrameworkCategoryRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(FormalFrameworkCategoryController), nameof(FormalFrameworkCategoryController.Get), new { id = internalMessage.FormalFrameworkCategoryId });
    }
}
