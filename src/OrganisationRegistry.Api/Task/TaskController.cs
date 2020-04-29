namespace OrganisationRegistry.Api.Task
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Day.Commands;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using OrganisationRegistry.Infrastructure.Commands;
    using Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;

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
        [OrganisationRegistryAuthorize(Roles = Roles.AutomatedTask + "," + Roles.Developer)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(
            [FromServices] Func<Owned<OrganisationRegistryContext>> contextFactory,
            [FromServices] IKboSync kboSync,
            [FromBody] TaskRequest task)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            switch (task.Type)
            {
                case TaskType.CheckIfDayHasPassed:
                    await CommandSender.Send(new CheckIfDayHasPassed());
                    break;

                case TaskType.RebuildProjection:
                    if (task.Params.Length != 1)
                        throw new RebuildProjectionRequiresANameException();

                    _logger.LogInformation("Projection Rebuild for {ProjectionName} requested.", task.Params[0]);
                    await CommandSender.Send(new RebuildProjection(task.Params[0]));
                    break;

                case TaskType.CompensatingAction:
                    if (task.Params.Length != 1)
                        throw new CompensatingActionRequiresANameException();

                    _logger.LogInformation("Requested execution of {CompensatingAction}.", task.Params[0]);
                    using (var context = contextFactory().Value)
                    {
                        switch (task.Params[0].ToLowerInvariant())
                        {
                            case "2017-05-18-fix-bodies":
                                CompensatingAction20170518FixBodies(context);
                                break;

                            case "2017-05-18-fix-body-seats":
                                CompensatingAction20170518FixBodySeats(context);
                                break;

                            case "2017-11-24-fix-body-mandates":
                                CompensatingAction20171124FixBodyMandates(context);
                                break;
                        }
                    }

                    break;

                case TaskType.SyncFromKbo:
                    using (var context = contextFactory().Value)
                    {
                        kboSync.SyncFromKbo(CommandSender, context, User);
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
                .Where(x => x.BodyNumber == null || x.BodyNumber == "")
                .Select(x => x.Id)
                .ToList();

            _logger.LogInformation("Fixing {NumberOfBodies} bodies.", bodiesInNeedOfFixing.Count);
            foreach (var bodyId in bodiesInNeedOfFixing)
                await CommandSender.Send(new AssignBodyNumber(new BodyId(bodyId)));
        }

        private async Task CompensatingAction20170518FixBodySeats(OrganisationRegistryContext context)
        {
            var seatsInNeedOfFixing = context
                .BodySeatList
                .AsQueryable()
                .Where(x => x.BodySeatNumber == null || x.BodySeatNumber == "")
                .ToList();

            _logger.LogInformation("Fixing {NumberOfBodySeats} body seats.", seatsInNeedOfFixing.Count);
            foreach (var bodySeat in seatsInNeedOfFixing)
                await CommandSender.Send(new AssignBodySeatNumber(new BodyId(bodySeat.BodyId), new BodySeatId(bodySeat.BodySeatId)));
        }

        private void CompensatingAction20171124FixBodyMandates(OrganisationRegistryContext context)
        {
            var mandatesInNeedOfFixing = context.BodyMandateList
                .FromSqlRaw(@"
                    SELECT A.*
                    FROM [OrganisationRegistry].[BodyMandateList] A
	                    INNER JOIN [OrganisationRegistry].[BodyMandateList] B
		                    ON A.BodyMandateId <> B.BodyMandateId AND
			                    A.BodyId = B.BodyId AND
			                    A.BodySeatId = B.BodySeatId AND
			                    (A.ValidFrom = B.ValidFrom OR (ISNULL(A.ValidFrom, B.ValidFrom) IS NULL)) AND
			                    (A.ValidTo = B.ValidTo OR (ISNULL(A.ValidTo, B.ValidTo) IS NULL))")
                .ToList();

            _logger.LogInformation("Fixing {NumberOfBodyMandates} body seats.", mandatesInNeedOfFixing.Count);
            foreach (var mandate in mandatesInNeedOfFixing)
            {

            }
        }
    }
}
