namespace OrganisationRegistry.Api.Organisation
{
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Responses;
    using Security;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations")]
    public class OrganisationKboController : OrganisationRegistryController
    {
        public OrganisationKboController(
            ICommandSender commandSender
            ) : base(commandSender)
        {
        }

        /// <summary>Couple an organisation to a KBO number.</summary>
        /// <response code="200">If the organisation was coupled.</response>
        [HttpPut("{id}/kbo/number/{kboNumber}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CoupleToKboNumber(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid id,
            [FromRoute] string kboNumber)
        {
            if (!securityService.CanEditOrganisation(User, id))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(
                new CoupleOrganisationToKbo(
                    new OrganisationId(id),
                    new KboNumber(kboNumber),
                    User));

            return Ok();
        }

        /// <summary>Cancel an organisation's active coupling with a KBO number.</summary>
        /// <response code="200">If the organisation coupling was cancelled.</response>
        [HttpDelete("{id}/kbo/cancel")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelCouplingWithKbo(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid id)
        {
            if (!securityService.CanEditOrganisation(User, id))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(
                new CancelCouplingWithKbo(
                    new OrganisationId(id),
                    User));

            return Ok();
        }

        /// <summary>Couple an organisation to a kbo number.</summary>
        /// <response code="200">If the organisation was coupled.</response>
        [HttpPut("{id}/kbo/terminate")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> TerminateKboCoupling(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid id)
        {
            if (!securityService.CanEditOrganisation(User, id))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(
                new TerminateKboCoupling(
                    new OrganisationId(id),
                    User));

            return Ok();
        }

        [HttpPut("{id}/kbo/sync")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateFromKbo(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid id)
        {
            if (!securityService.CanEditOrganisation(User, id))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(
                new UpdateFromKbo(
                    new OrganisationId(id),
                    User, DateTimeOffset.Now,
                    null));

            return Ok();
        }

        /// <summary>Couple an organisation to a kbo number.</summary>
        /// <response code="200">If the organisation was coupled.</response>
        [HttpGet("{id}/kbo/{kboNumber}/termination")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTerminationStatus(
            [FromServices] ISecurityService securityService,
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] string kboNumber,
            [FromRoute] Guid id)
        {
            if (!securityService.CanEditOrganisation(User, id))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            var organisationTermination = await context.OrganisationTerminationList.SingleOrDefaultAsync(x => x.Id == id && x.KboNumber == kboNumber);

            if (organisationTermination == null)
                return Ok(OrganisationTerminationResponse.NotFound(id));

            return Ok(OrganisationTerminationResponse.FromListItem(organisationTermination));
        }

        /// <summary>Get a list of events.</summary>
        [HttpGet("kbo/terminated")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.Developer)]
        [ProducesResponseType(typeof(List<OrganisationTerminationResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationTerminationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedFunctions = new OrganisationTerminationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedFunctions.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedFunctions.Items.ToListAsync());
        }
    }
}
