namespace OrganisationRegistry.Api.Backoffice.Organisation
{
    using System;
    using System.Threading.Tasks;
    using Handling.Authorization;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/regulations")]
    [FeatureGate(FeatureFlags.RegulationsManagement)]
    public class OrganisationRegulationController : OrganisationRegistryController
    {
        public OrganisationRegulationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available regulations for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationRegulationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var user = await securityService.GetUser(User);
            var isAuthorizedForRegulation = () =>
                new RegulationPolicy()
                    .Check(user)
                    .IsSuccessful;

            var pagedOrganisations = new OrganisationRegulationListQuery(
                context,
                organisationId,
                isAuthorizedForRegulation)
                .Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a regulation for an organisation.</summary>
        /// <response code="200">If the regulation is found.</response>
        /// <response code="404">If the regulation cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationRegulationList.FirstOrDefaultAsync(x => x.OrganisationRegulationId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Create a regulation for an organisation.</summary>
        /// <response code="201">If the regulation is created, together with the location.</response>
        /// <response code="400">If the regulation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(
            [FromRoute] Guid organisationId,
            [FromBody] AddOrganisationRegulationRequest message)
        {
            var internalMessage = new AddOrganisationRegulationInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationRegulationRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.OrganisationRegulationId });
        }

        /// <summary>Update a regulation for an organisation.</summary>
        /// <response code="201">If the regulation is updated, together with the location.</response>
        /// <response code="400">If the regulation information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(
            [FromRoute] Guid organisationId,
            [FromBody] UpdateOrganisationRegulationRequest message)
        {
            var internalMessage = new UpdateOrganisationRegulationInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationRegulationRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
