namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration
{
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using Security;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("configuration")]
    [OrganisationRegistryAuthorize(Roles = Roles.Developer)]
    public class ConfigurationController : OrganisationRegistryController
    {
        public ConfigurationController(ICommandSender commandSender) : base(commandSender)
        {
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] ConfigurationContext context, [FromRoute] string id)
        {
            var configurationValue =
                await context.Configuration
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.Key == id);

            if (configurationValue == null)
                return NotFound();

            return Ok(configurationValue);
        }

        /// <summary>Create a configuration value.</summary>
        /// <response code="201">If the configuration value is created, together with the location.</response>
        /// <response code="400">If the configuration value does not pass validation.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromServices] ConfigurationContext context, [FromBody] CreateConfigurationValueRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            context.Configuration.Add(new ConfigurationValue(message.Key, message.Description, message.Value));
            await context.SaveChangesAsync();

            return Created(Url.Action(nameof(Get), new { id = message.Key }) ?? "", null);
        }

        /// <summary>Update a configuration value.</summary>
        /// <response code="200">If the configuration value is updated, together with the location.</response>
        /// <response code="400">If the configuration value does not pass validation.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromServices] ConfigurationContext context, [FromRoute] string id, [FromBody] UpdateConfigurationValueRequest message)
        {
            var internalMessage = new UpdateConfigurationValueInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            var configurationValue = context.Configuration.Single(x => x.Key == id);
            configurationValue.Value = internalMessage.Body.Value;
            configurationValue.Description = internalMessage.Body.Description;
            await context.SaveChangesAsync();

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.Key });
        }
    }
}
