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
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Queries;
    using Requests;
    using Responses;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/capacities")]
    public class OrganisationCapacityController : OrganisationRegistryController
    {
        public OrganisationCapacityController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available capacities for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IOrganisationRegistryConfiguration configuration,
            [FromServices] IMemoryCaches memoryCaches,
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationCapacityListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var user = await securityService.GetUser(User);
            Func<Guid, bool> isAuthorizedForCapacity = id =>
                new CapacityPolicy(
                        memoryCaches.OvoNumbers[organisationId],
                        configuration,
                        id)
                    .Check(user)
                    .IsSuccessful;

            var pagedOrganisations =
                new OrganisationCapacityListQuery(
                    context,
                    organisationId,
                    isAuthorizedForCapacity)
                    .Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a capacity for an organisation.</summary>
        /// <response code="200">If the capacity is found.</response>
        /// <response code="404">If the capacity cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationCapacityList.FirstOrDefaultAsync(x => x.OrganisationCapacityId == id);

            if (organisation == null)
                return NotFound();

            return Ok(new OrganisationCapacityResponse(organisation));
        }

        /// <summary>Create a capacity for an organisation.</summary>
        /// <response code="201">If the capacity is created, together with the location.</response>
        /// <response code="400">If the capacity information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationCapacityRequest message)
        {
            var internalMessage = new AddOrganisationCapacityInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationCapacityRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.OrganisationCapacityId });
        }

        /// <summary>Update a capacity for an organisation.</summary>
        /// <response code="201">If the capacity is updated, together with the location.</response>
        /// <response code="400">If the capacity information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationCapacityRequest message)
        {
            var internalMessage = new UpdateOrganisationCapacityInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationCapacityRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
