namespace OrganisationRegistry.Api.FunctionType
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Security;
    using SqlServer.FunctionType;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("functiontypes")]
    public class FunctionTypeController : OrganisationRegistryController
    {
        public FunctionTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available function types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<FunctionTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedFunctionTypes = new FunctionTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedFunctionTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedFunctionTypes.Items.ToListAsync());
        }

        /// <summary>Get a function type.</summary>
        /// <response code="200">If the function type is found.</response>
        /// <response code="404">If the function type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FunctionTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var functionTypeListItem = await context.FunctionTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (functionTypeListItem == null)
                return NotFound();

            return Ok(functionTypeListItem);
        }

        /// <summary>Create a function type.</summary>
        /// <response code="201">If the function type is created, together with the location.</response>
        /// <response code="400">If the function type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateFunctionTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateFunctionTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a function type.</summary>
        /// <response code="200">If the function type is updated, together with the location.</response>
        /// <response code="400">If the function type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateFunctionTypeRequest message)
        {
            var internalMessage = new UpdateFunctionTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateFunctionTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.FunctionId }));
        }
    }
}
