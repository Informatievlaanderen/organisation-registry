namespace OrganisationRegistry.Api.Backoffice.Organisation.OrganisationClassification;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/classifications")]
public class OrganisationOrganisationClassificationController : OrganisationRegistryController
{
    public OrganisationOrganisationClassificationController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available classifications for an organisation.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationOrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForOrganisationClassificationType = id =>
            new OrganisationClassificationTypePolicy(
                    memoryCaches.OvoNumbers[organisationId],
                    configuration,
                    id)
                .Check(user)
                .IsSuccessful;

        var pagedOrganisations =
            new OrganisationOrganisationClassificationListQuery(
                    context,
                    organisationId,
                    isAuthorizedForOrganisationClassificationType)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a classification for an organisation.</summary>
    /// <response code="200">If the classification is found.</response>
    /// <response code="404">If the classification cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationOrganisationClassificationList.FirstOrDefaultAsync(x => x.OrganisationOrganisationClassificationId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }

    /// <summary>Create a classification for an organisation.</summary>
    /// <response code="201">If the classification is created, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationOrganisationClassificationRequest message)
    {
        var internalMessage = new AddOrganisationOrganisationClassificationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationOrganisationClassificationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(Get), new { id = message.OrganisationOrganisationClassificationId });
    }

    /// <summary>Update a classification for an organisation.</summary>
    /// <response code="201">If the classification is updated, together with the location.</response>
    /// <response code="400">If the classification information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationOrganisationClassificationRequest message)
    {
        var internalMessage = new UpdateOrganisationOrganisationClassificationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationOrganisationClassificationRequestMapping.Map(internalMessage));

        return Ok();
    }
}