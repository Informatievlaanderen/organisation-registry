namespace OrganisationRegistry.Api.Backoffice.Admin.Projections;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize(Role.Developer)]
[OrganisationRegistryRoute("projections")]
public class ProjectionsCommandController : OrganisationRegistryController
{
    public ProjectionsCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    [HttpPut("states/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromServices] OrganisationRegistryContext context, Guid id, [FromBody] UpdateProjectionStateRequest message)
    {
        var state = await context.ProjectionStates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (state == null)
            return NotFound();

        state.Name = message.Name;
        state.EventNumber = message.EventNumber;

        await context.SaveChangesAsync();

        return OkWithLocationHeader(nameof(ProjectionsController), nameof(ProjectionsController.Get), new { id });
    }
}
