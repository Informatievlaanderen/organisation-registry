namespace OrganisationRegistry.Api.Backoffice.Body.FormalFramework;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/formalframeworks")]
public class BodyFormalFrameworkController : OrganisationRegistryController
{
    public BodyFormalFrameworkController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available formal frameworks for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyFormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyFormalFrameworks = new BodyFormalFrameworkListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Get a formal framework for a body.</summary>
    /// <response code="200">If the formal framework is found.</response>
    /// <response code="404">If the formal framework cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyFormalFramework = await context.BodyFormalFrameworkList.FirstOrDefaultAsync(x => x.BodyFormalFrameworkId == id);

        if (bodyFormalFramework == null)
            return NotFound();

        return Ok(bodyFormalFramework);
    }

    /// <summary>Create a formal framework for a body.</summary>
    /// <response code="201">If the formal framework is created, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyFormalFrameworkRequest message)
    {
        var internalMessage = new AddBodyFormalFrameworkInternalRequest(bodyId, message);

        if (!await securityService.CanEditBody(User, internalMessage.BodyId))
            ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddBodyFormalFrameworkRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(Get), new { id = message.BodyFormalFrameworkId });
    }

    /// <summary>Update a formal framework for a body.</summary>
    /// <response code="201">If the formal framework is updated, together with the location.</response>
    /// <response code="400">If the formal framework information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyFormalFrameworkRequest message)
    {
        var internalMessage = new UpdateBodyFormalFrameworkInternalRequest(bodyId, message);

        if (!await securityService.CanEditBody(User, internalMessage.BodyId))
            ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateBodyFormalFrameworkRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.BodyId });
    }
}
