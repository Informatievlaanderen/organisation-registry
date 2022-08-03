namespace OrganisationRegistry.Api.Backoffice.Admin.Projections;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using Requests;
using SqlServer;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize(Role.Developer)]
[OrganisationRegistryRoute("projections")]
public class ProjectionsController : OrganisationRegistryController
{
    /// <summary>Get a list of projections.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var projections = typeof(OrganisationRegistrySqlServerAssemblyTokenClass)
            .GetTypeInfo()
            .Assembly
            .GetTypes()
            .Where(x =>
            {
                var typeInfo = x.GetTypeInfo();
                return
                    typeInfo.IsClass &&
                    !typeInfo.IsAbstract &&
                    typeInfo.ImplementedInterfaces.Any(y => y == typeof(IProjectionMarker));
            })
            .Select(x => x.FullName)
            .OrderBy(x => x);

        return await OkAsync(projections);
    }

    /// <summary>Get a list of available projection states.</summary>
    [HttpGet("states")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        => Ok(await context.ProjectionStates
            .AsQueryable()
            .ToListAsync());

    /// <summary>Get a projection state by its id.</summary>
    [HttpGet("states/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, Guid id)
    {
        var state = await context.ProjectionStates
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (state == null)
            return NotFound();

        return Ok(state);
    }

    /// <summary>Get the max event number.</summary>
    [HttpGet("states/last-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LastEvent([FromServices] OrganisationRegistryContext context)
    {
        return Ok(await context.Events.AsQueryable().MaxAsync(x => x.Number));
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
