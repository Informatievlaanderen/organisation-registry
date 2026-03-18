namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType;

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
[OrganisationRegistryRoute("functiontypes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class FunctionTypeCommandController : OrganisationRegistryCommandController
{
    public FunctionTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een functietype.</summary>
    /// <response code="201">Als het functietype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het functietype mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateFunctionTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateFunctionTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(FunctionTypeController), nameof(FunctionTypeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een functietype aan.</summary>
    /// <response code="200">Als het functietype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het functietype mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFunctionTypeRequest message)
    {
        var internalMessage = new UpdateFunctionTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateFunctionTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(FunctionTypeController), nameof(FunctionTypeController.Get), new { id = internalMessage.FunctionId });
    }
}
