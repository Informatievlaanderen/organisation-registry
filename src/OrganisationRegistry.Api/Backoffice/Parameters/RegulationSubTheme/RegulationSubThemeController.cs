namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme;

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
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("regulationsubthemes")]
public class RegulationSubThemeController : OrganisationRegistryController
{
    public RegulationSubThemeController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available regulation sub-themes.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<RegulationSubThemeListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedRegulationSubThemes = new RegulationSubThemeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedRegulationSubThemes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedRegulationSubThemes.Items.ToListAsync());
    }

    /// <summary>Get an regulation sub-theme.</summary>
    /// <response code="200">If the regulation sub-theme is found.</response>
    /// <response code="404">If the regulation sub-theme cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var regulationSubTheme = await context.RegulationSubThemeList.FirstOrDefaultAsync(x => x.Id == id);

        if (regulationSubTheme == null)
            return NotFound();

        return Ok(regulationSubTheme);
    }

    /// <summary>Create an regulation sub-theme.</summary>
    /// <response code="201">If the regulation sub-theme is created, together with the location.</response>
    /// <response code="400">If the regulation sub-theme information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateRegulationSubThemeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateRegulationSubThemeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update an regulation sub-theme.</summary>
    /// <response code="200">If the regulation sub-theme is updated, together with the location.</response>
    /// <response code="400">If the regulation sub-theme information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRegulationSubThemeRequest message)
    {
        var internalMessage = new UpdateRegulationSubThemeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateRegulationSubThemeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.RegulationSubThemeId });
    }
}