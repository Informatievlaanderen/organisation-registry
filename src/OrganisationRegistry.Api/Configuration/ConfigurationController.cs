namespace OrganisationRegistry.Api.Configuration
{
    using System.Linq;
    using System.Net;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using OrganisationRegistry.Infrastructure.Commands;
    using System.Threading.Tasks;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Queries;
    using Requests;
    using Security;
    using OrganisationRegistry.Configuration.Database;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("configuration")]
    [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
    public class ConfigurationController : OrganisationRegistryController
    {
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(ICommandSender commandSender, ILogger<ConfigurationController> logger) : base(commandSender)
        {
            _logger = logger;
        }

        /// <summary>Get a list of available configuration values.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] ConfigurationContext context)
        {
            var filtering = Request.ExtractFilteringRequest<ConfigurationValue>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedConfiguration = new ConfigurationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedConfiguration.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedConfiguration.Items.ToListAsync());
        }

        /// <summary>Get a configuration value.</summary>
        /// <response code="200">If the configuration value is found.</response>
        /// <response code="404">If the configuration value cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConfigurationValue), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] ConfigurationContext context, [FromRoute] string id)
        {
            var configurationValue = await context.Configuration.FirstOrDefaultAsync(x => x.Key == id);

            if (configurationValue == null)
                return NotFound();

            return Ok(configurationValue);
        }

        /// <summary>Create a configuration value.</summary>
        /// <response code="201">If the configuration value is created, together with the location.</response>
        /// <response code="400">If the configuration value does not pass validation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromServices] ConfigurationContext context, [FromBody] CreateConfigurationValueRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Configuration.Add(new ConfigurationValue(message.Key, message.Description, message.Value));
            context.SaveChanges();

            return Created(Url.Action(nameof(Get), new { id = message.Key }), null);
        }

        /// <summary>Update a configuration value.</summary>
        /// <response code="200">If the configuration value is updated, together with the location.</response>
        /// <response code="400">If the configuration value does not pass validation.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromServices] ConfigurationContext context, [FromRoute] string id, [FromBody] UpdateConfigurationValueRequest message)
        {
            var internalMessage = new UpdateConfigurationValueInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            var configurationValue = context.Configuration.Single(x => x.Key == id);
            configurationValue.Value = internalMessage.Body.Value;
            configurationValue.Description = internalMessage.Body.Description;
            context.SaveChanges();

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.Key }));
        }
    }
}
