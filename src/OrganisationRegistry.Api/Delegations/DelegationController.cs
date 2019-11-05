namespace OrganisationRegistry.Api.Delegations
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
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
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("manage/delegations")]
    public class DelegationController : OrganisationRegistryController
    {
        public DelegationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available delegations.</summary>
        [HttpGet]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.OrganisatieBeheerder)]
        [ProducesResponseType(typeof(List<DelegationListQueryResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromServices] ISecurityService securityService)
        {
            var filtering = Request.ExtractFilteringRequest<DelegationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var securityInformationFunc = new Func<SecurityInformation>(() =>
            {
                var authInfo = HttpContext.GetAuthenticateInfo();
                return securityService.GetSecurityInformation(authInfo.Principal);
            });

            var pagedDelegations =
                new DelegationListQuery(context, securityInformationFunc).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedDelegations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedDelegations.Items.ToListAsync());
        }

        /// <summary>Get a delegation.</summary>
        /// <response code="200">If the delegation is found.</response>
        /// <response code="404">If the delegation cannot be found.</response>
        [HttpGet("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.OrganisatieBeheerder)]
        [ProducesResponseType(typeof(DelegationResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] ISecurityService securityService, [FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == id);

            if (delegation == null)
                return NotFound();

            if (!securityService.CanEditDelegation(User, delegation.OrganisationId, delegation.BodyId))
                return Unauthorized(); // ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze delegatie.");

            return Ok(new DelegationResponse(delegation));
        }
    }
}
