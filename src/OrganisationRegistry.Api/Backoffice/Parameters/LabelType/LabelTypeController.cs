namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType;

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
using Queries;
using Requests;
using Security;
using SqlServer.Infrastructure;
using SqlServer.LabelType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("labeltypes")]
public class LabelTypeController : OrganisationRegistryController
{
    public LabelTypeController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available label types.</summary>
    [HttpGet]
    [OrganisationRegistryAuthorize]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromQuery] Guid? forOrganisationId)
    {
        var filtering = Request.ExtractFilteringRequest<LabelTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForLabelType = labelTypeId =>
            !forOrganisationId.HasValue ||
            LabelPolicy.ForCreate(
                    memoryCaches.OvoNumbers[forOrganisationId.Value],
                    memoryCaches.UnderVlimpersManagement.Contains(forOrganisationId.Value),
                    configuration,
                    labelTypeId)
                .Check(user)
                .IsSuccessful;

        var pagedLabelTypes = new LabelTypeListQuery(context, configuration, isAuthorizedForLabelType).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLabelTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLabelTypes.Items.ToListAsync());
    }

    /// <summary>Get a label type.</summary>
    /// <response code="200">If the label type is found.</response>
    /// <response code="404">If the label type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid id)
    {
        var key = await context.LabelTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }

    /// <summary>Create a label type.</summary>
    /// <response code="201">If the label type is created, together with the location.</response>
    /// <response code="400">If the label type information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateLabelTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateLabelTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update a label type.</summary>
    /// <response code="200">If the label type is updated, together with the location.</response>
    /// <response code="400">If the label type information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLabelTypeRequest message)
    {
        var internalMessage = new UpdateLabelTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateLabelTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.LabelTypeId });
    }
}
