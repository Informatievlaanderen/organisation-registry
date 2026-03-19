namespace OrganisationRegistry.Api.Backoffice.Admin.Projections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using Requests;
using SqlServer;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize(Role.Developer)]
[OrganisationRegistryRoute("projections")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Administratie")]
public class ProjectionsController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van projecties op.</summary>
    /// <response code="200">Een lijst van projecties.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProjectionNameListExamples))]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
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

    /// <summary>Vraag een lijst van projectiestatussen op.</summary>
    /// <response code="200">Een lijst van projectiestatussen.</response>
    [HttpGet("states")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProjectionStateListExamples))]
    [ProducesResponseType(typeof(List<ProjectionStateItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        => Ok(await context.ProjectionStates
            .AsQueryable()
            .ToListAsync());

    /// <summary>Vraag een projectiestatus op basis van het id op.</summary>
    /// <response code="200">Een lijst van projectiestatussen.</response>
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

    /// <summary>Vraag het sequentienummer op van het meest recente event.</summary>
    /// <response code="200">Het sequentienummer van het meest recente event.</response>
    [HttpGet("states/last-event")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LastEvent([FromServices] OrganisationRegistryContext context)
    {
        return Ok(await context.Events.AsQueryable().MaxAsync(x => x.Number));
    }

    /// <summary>Pas een projectiestatus aan.</summary>
    /// <response code="200">Als de projectiestatus succesvol aangepast is.</response>
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
