namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration;

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
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Queries;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("configuration")]
[OrganisationRegistryAuthorize(Role.Developer)]
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
}
