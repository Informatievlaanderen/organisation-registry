namespace OrganisationRegistry.Api.FormalFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Configuration;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Queries;
    using Requests;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Security;
    using SqlServer.FormalFramework;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("formalframeworks")]
    public class FormalFrameworkController : OrganisationRegistryController
    {
        private readonly IOptions<ApiConfiguration> _config;

        public FormalFrameworkController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config)
            : base(commandSender)
        {
            _config = config;
        }

        /// <summary>Get a list of available formal frameworks.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<FormalFrameworkListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedFormalFrameworks = new FormalFrameworkListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedFormalFrameworks.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedFormalFrameworks.Items.ToListAsync());
        }

        /// <summary>Get a list of available formal frameworks for vademecum report.</summary>
        [HttpGet("vademecum")]
        public async Task<IActionResult> GetVademecumFormalFrameworks([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<FormalFrameworkListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            if (!_config.Value.VademecumReport_FormalFrameworkIds.IsNullOrWhiteSpace())
                foreach (var id in _config.Value.VademecumReport_FormalFrameworkIds.Split(new[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries))
                    if (Guid.TryParse(id, out Guid guid))
                        filtering.Filter.Ids.Add(guid);

            var pagedFormalFrameworks = new FormalFrameworkListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedFormalFrameworks.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedFormalFrameworks.Items.ToListAsync());
        }

        /// <summary>Get a formal framework.</summary>
        /// <response code="200">If the formal framework is found.</response>
        /// <response code="404">If the formal framework cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormalFrameworkListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var formalFramework = await context.FormalFrameworkList.FirstOrDefaultAsync(x => x.Id == id);

            if (formalFramework == null)
                return NotFound();

            return Ok(formalFramework);
        }

        /// <summary>Create a formal framework.</summary>
        /// <response code="201">If the formal framework is created, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateFormalFrameworkRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateFormalFrameworkRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a formal framework.</summary>
        /// <response code="200">If the formal framework is updated, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkRequest message)
        {
            var internalMessage = new UpdateFormalFrameworkInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateFormalFrameworkRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.FormalFrameworkId }));
        }
    }
}
