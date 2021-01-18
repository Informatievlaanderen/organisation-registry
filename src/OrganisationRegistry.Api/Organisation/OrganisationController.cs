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
    using Requests;
    using Responses;
    using Security;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations")]
    public class OrganisationController : OrganisationRegistryController
    {
        public OrganisationController(ICommandSender commandSender) : base(commandSender)
        {
        }

        /// <summary>Get a list of available organisations.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrganisationListQueryResult>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromServices] ISecurityService securityService)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var securityInformationFunc = new Func<SecurityInformation>(() =>
            {
                var authInfo = HttpContext.GetAuthenticateInfo();
                return securityService.GetSecurityInformation(authInfo.Principal);
            });

            var pagedOrganisations =
                new OrganisationListQuery(context, securityInformationFunc)
                    .Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get an organisation.</summary>
        /// <response code="200">If the organisation is found.</response>
        /// <response code="404">If the organisation cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.Id == id);

            if (organisation == null)
                return NotFound();

            return Ok(new OrganisationResponse(organisation));
        }

        /// <summary>Create an organisation.</summary>
        /// <response code="201">If the organisation is created, together with the location.</response>
        /// <response code="400">If the organisation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(
            [FromServices] ISecurityService securityService,
            [FromBody] CreateOrganisationRequest message)
        {
            if (!securityService.CanAddOrganisation(User, message.ParentOrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authInfo = await HttpContext.GetAuthenticateInfoAsync();
            if (authInfo.Principal == null || !authInfo.Principal.IsInRole(Roles.Developer))
                message.OvoNumber = string.Empty;

            if (!string.IsNullOrWhiteSpace(message.KboNumber))
            {
                await CommandSender.Send(CreateOrganisationRequestMapping.MapToCreateKboOrganisation(message, User));
            }
            else
                await CommandSender.Send(CreateOrganisationRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update an organisation.</summary>
        /// <response code="200">If the organisation is updated, together with the location.</response>
        /// <response code="400">If the organisation information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid id, [FromBody] UpdateOrganisationInfoRequest message)
        {
            var internalMessage = new UpdateOrganisationInfoInternalRequest(id, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationInfoRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.OrganisationId }));
        }

        /// <summary>Terminate an organisation.</summary>
        /// <response code="200">If the organisation is terminated, together with the location.</response>
        [HttpDelete("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {

            await CommandSender.Send(new TerminateOrganisation(
                new OrganisationId(id),
                DateTime.Today,
                User));

            // TODO: fix roles & date

            return Ok();
        }

    }
}
