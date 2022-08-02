namespace OrganisationRegistry.Api.Backoffice.Admin.Task;

using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.OwnedInstances;
using Exceptions;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("tasks")]
public class TaskController : OrganisationRegistryController
{
    private readonly ILogger<TaskController> _logger;

    public TaskController(ICommandSender commandSender, ILogger<TaskController> logger) : base(commandSender)
    {
        _logger = logger;
    }

    /// <summary>Executes a task.</summary>
    /// <response code="200">If the task was performed.</response>
    /// <response code="400">If the task information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Role.AutomatedTask,Role.Developer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] Func<Owned<OrganisationRegistryContext>> contextFactory,
        [FromBody] TaskRequest task)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        switch (task.Type)
        {
            case TaskType.RebuildProjection:
                if (task.Params.Length != 1)
                    throw new RebuildProjectionRequiresAName();

                _logger.LogInformation("Projection Rebuild for {ProjectionName} requested", task.Params[0]);
                await CommandSender.Send(new RebuildProjection(task.Params[0]));
                break;

            case TaskType.CompensatingAction:
                if (task.Params.Length != 1)
                    throw new CompensatingActionRequiresAName();

                _logger.LogInformation("Requested execution of {CompensatingAction}", task.Params[0]);
                await using (var context = contextFactory().Value)
                {
                    switch (task.Params[0].ToLowerInvariant())
                    {
                        case "2017-05-18-fix-bodies":
                            await CompensatingAction20170518FixBodies(context);
                            break;

                        case "2017-05-18-fix-body-seats":
                            await CompensatingAction20170518FixBodySeats(context);
                            break;
                    }
                }

                break;

            default:
                return BadRequest(ModelState);
        }

        return Ok();
    }

    private async Task CompensatingAction20170518FixBodies(OrganisationRegistryContext context)
    {
        var bodiesInNeedOfFixing = context
            .BodyDetail
            .AsQueryable()
            .Where(x => x.BodyNumber == "")
            .Select(x => x.Id)
            .ToList();

        _logger.LogInformation("Fixing {NumberOfBodies} bodies", bodiesInNeedOfFixing.Count);
        foreach (var bodyId in bodiesInNeedOfFixing)
            await CommandSender.Send(new AssignBodyNumber(new BodyId(bodyId)));
    }

    private async Task CompensatingAction20170518FixBodySeats(OrganisationRegistryContext context)
    {
        var seatsInNeedOfFixing = context
            .BodySeatList
            .AsQueryable()
            .Where(x => x.BodySeatNumber == "")
            .ToList();

        _logger.LogInformation("Fixing {NumberOfBodySeats} body seats", seatsInNeedOfFixing.Count);
        foreach (var bodySeat in seatsInNeedOfFixing)
            await CommandSender.Send(new AssignBodySeatNumber(new BodyId(bodySeat.BodyId), new BodySeatId(bodySeat.BodySeatId)));
    }
}
