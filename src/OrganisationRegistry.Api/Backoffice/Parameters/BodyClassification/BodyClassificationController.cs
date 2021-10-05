namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.BodyClassification;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodyclassifications")]
    public class BodyClassificationController : OrganisationRegistryController
    {
        private readonly ApiConfiguration _config;

        public BodyClassificationController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config)
            : base(commandSender)
        {
            _config = config.Value;
        }

        /// <summary>Get a list of available body classifications.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<BodyClassificationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodyClassifications = new BodyClassificationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodyClassifications.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodyClassifications.Items.ToListAsync());
        }

        /// <summary>Get an body classification.</summary>
        /// <response code="200">If the body classification is found.</response>
        /// <response code="404">If the body classification cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var bodyClassification = await context.BodyClassificationList.FirstOrDefaultAsync(x => x.Id == id);

            if (bodyClassification == null)
                return NotFound();

            return Ok(bodyClassification);
        }

        /// <summary>Create an body classification.</summary>
        /// <response code="201">If the body classificiation is created, together with the location.</response>
        /// <response code="400">If the body classificiation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateBodyClassificationRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateBodyClassificationRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update an body classification.</summary>
        /// <response code="200">If the body classification is updated, together with the location.</response>
        /// <response code="400">If the body classification information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyClassificationRequest message)
        {
            var internalMessage = new UpdateBodyClassificationInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyClassificationRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyClassificationId }));
        }
    }
}
