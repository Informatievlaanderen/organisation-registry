namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
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
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("formalframeworks")]
    public class FormalFrameworkController : OrganisationRegistryController
    {
        private readonly IOptions<ApiConfigurationSection> _config;

        public FormalFrameworkController(
            ICommandSender commandSender,
            IOptions<ApiConfigurationSection> config)
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

            if (filtering.Filter is not { })
                filtering.Filter = new FormalFrameworkListItemFilter();

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var formalFramework = await context.FormalFrameworkList
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (formalFramework == null)
                return NotFound();

            return Ok(formalFramework);
        }

        /// <summary>Create a formal framework.</summary>
        /// <response code="201">If the formal framework is created, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateFormalFrameworkRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateFormalFrameworkRequestMapping.Map(message));

            return CreatedWithLocation(nameof(Get), new { id = message.Id });
        }

        /// <summary>Update a formal framework.</summary>
        /// <response code="200">If the formal framework is updated, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateFormalFrameworkRequest message)
        {
            var internalMessage = new UpdateFormalFrameworkInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateFormalFrameworkRequestMapping.Map(internalMessage));

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.FormalFrameworkId });
        }
    }
}
