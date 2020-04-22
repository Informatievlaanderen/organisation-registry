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
    using System.Net;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/functions")]
    public class OrganisationFunctionController : OrganisationRegistryController
    {
        public OrganisationFunctionController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available functions for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationFunctionListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationFunctionListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a function for an organisation.</summary>
        /// <response code="200">If the function is found.</response>
        /// <response code="404">If the function cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationFunctionResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationFunctionList.FirstOrDefaultAsync(x => x.OrganisationFunctionId == id);

            if (organisation == null)
                return NotFound();

            return Ok(new OrganisationFunctionResponse(organisation));
        }

        /// <summary>Create a function for an organisation.</summary>
        /// <response code="201">If the function is created, together with the location.</response>
        /// <response code="400">If the function information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] AddOrganisationFunctionRequest message)
        {
            var internalMessage = new AddOrganisationFunctionInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationFunctionRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.OrganisationFunctionId }), null);
        }

        /// <summary>Update a function for an organisation.</summary>
        /// <response code="201">If the function is updated, together with the location.</response>
        /// <response code="400">If the function information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFunctionRequest message)
        {
            var internalMessage = new UpdateOrganisationFunctionInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationFunctionRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
