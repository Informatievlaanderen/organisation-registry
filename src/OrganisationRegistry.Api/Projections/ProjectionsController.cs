namespace OrganisationRegistry.Api.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Requests;
    using Security;
    using SqlServer;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("projections")]
    public class ProjectionsController : OrganisationRegistryController
    {
        public ProjectionsController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of projections.</summary>
        [HttpGet]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
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
                        typeInfo.ImplementedInterfaces.Any(y => y == typeof(SqlServer.Infrastructure.IProjectionMarker));
                })
                .Select(x => x.FullName)
                .OrderBy(x => x);

            return Ok(projections);
        }

        /// <summary>Get a list of available projection states.</summary>
        [HttpGet("states")]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        [ProducesResponseType(typeof(IEnumerable<ProjectionStateItem>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            return Ok(await context.ProjectionStates
                .AsQueryable()
                .ToListAsync());
        }

        /// <summary>Get a projection state by its id.</summary>
        [HttpGet("states/{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        [ProducesResponseType(typeof(ProjectionStateItem), (int)HttpStatusCode.OK)]
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
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> LastEvent([FromServices] OrganisationRegistryContext context)
        {
            return Ok(await context.Events
                .AsQueryable()
                .MaxAsync(x => x.Number));
        }

        [HttpPut("states/{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] OrganisationRegistryContext context, Guid id, [FromBody] UpdateProjectionStateRequest message)
        {
            var state = await context.ProjectionStates
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (state == null)
                return NotFound();

            state.Name = message.Name;
            state.EventNumber = message.EventNumber;

            context.SaveChanges();

            return OkWithLocation(Url.Action(nameof(Get), new { id }));
        }
    }
}
