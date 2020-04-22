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
    using Security;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/openingHours")]
    public class OrganisationOpeningHourController : OrganisationRegistryController
    {
        public OrganisationOpeningHourController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationOpeningHourListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationOpeningHourListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationOpeningHourListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] Guid organisationId,
            [FromRoute] Guid id)
        {
            var openingHour = await context.OrganisationOpeningHourList.FirstOrDefaultAsync(x => x.OrganisationOpeningHourId == id);
            if (openingHour == null)
                return NotFound();

            return Ok(openingHour);
        }

        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid organisationId,
            [FromBody] AddOrganisationOpeningHourRequest message)
        {
            var internalMessage = new AddOrganisationOpeningHourInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationOpeningHourRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.OrganisationOpeningHourId }), null);
        }

        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put(
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid organisationId,
            [FromBody] UpdateOrganisationOpeningHourRequest message)
        {
            var internalMessage = new UpdateOrganisationOpeningHourInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationOpeningHourRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
