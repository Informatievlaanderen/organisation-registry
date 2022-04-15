namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFrameworkCategory
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.FormalFrameworkCategory;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("formalframeworkcategories")]
    public class FormalFrameworkCategoryController : OrganisationRegistryController
    {
        public FormalFrameworkCategoryController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available formal framework categories.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<FormalFrameworkCategoryListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedFormalFrameworkCategories = new FormalFrameworkCategoryListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedFormalFrameworkCategories.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedFormalFrameworkCategories.Items.ToListAsync());
        }

        /// <summary>Get a formal framework category.</summary>
        /// <response code="200">If the formal framework category is found.</response>
        /// <response code="404">If the formal framework category cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var formalFrameworkCategory = await context.FormalFrameworkCategoryList.FirstOrDefaultAsync(x => x.Id == id);

            if (formalFrameworkCategory == null)
                return NotFound();

            return Ok(formalFrameworkCategory);
        }

        /// <summary>Create a formal framework category.</summary>
        /// <response code="201">If the formal framework category is created, together with the location.</response>
        /// <response code="400">If the formal framework category information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateFormalFrameworkCategoryRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateFormalFrameworkCategoryRequestMapping.Map(message));

            return CreatedWithLocation(nameof(Get), new { id = message.Id });
        }

        /// <summary>Update a formal framework category.</summary>
        /// <response code="200">If the formal framework category is updated, together with the location.</response>
        /// <response code="400">If the formal framework category information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkCategoryRequest message)
        {
            var internalMessage = new UpdateFormalFrameworkCategoryInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateFormalFrameworkCategoryRequestMapping.Map(internalMessage));

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.FormalFrameworkCategoryId });
        }
    }
}
