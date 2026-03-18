namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration;

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
using OrganisationRegistry.Infrastructure.Authorization;
using Queries;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("configuration")]
[OrganisationRegistryAuthorize(Role.Developer)]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Administratie")]
public class ConfigurationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van configuratiewaarden op.</summary>
    /// <response code="200">Een lijst van configuratiewaarden.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    /// <summary>Vraag een configuratiewaarde op.</summary>
    /// <response code="200">Als de configuratiewaarde gevonden is.</response>
    /// <response code="404">Als de configuratiewaarde niet gevonden kan worden.</response>
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

    /// <summary>Registreer een configuratiewaarde.</summary>
    /// <response code="201">Als de configuratiewaarde succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de configuratiewaarde mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] ConfigurationContext context, [FromBody] CreateConfigurationValueRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        context.Configuration.Add(new ConfigurationValue(message.Key, message.Description, message.Value));
        await context.SaveChangesAsync();

        return CreatedWithLocation(nameof(ConfigurationController), nameof(Get), new { id = message.Key });
    }

    /// <summary>Pas een configuratiewaarde aan.</summary>
    /// <response code="200">Als de configuratiewaarde succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de configuratiewaarde mislukt is.</response>
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

        return OkWithLocationHeader(nameof(ConfigurationController), nameof(Get), new { id = internalMessage.Key });
    }
}
