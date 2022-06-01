namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType;

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
using SqlServer.MandateRoleType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("mandateroletypes")]
public class MandateRoleTypeController : OrganisationRegistryController
{
    public MandateRoleTypeController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available mandate role types.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<MandateRoleTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedMandateRoleTypes = new MandateRoleTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedMandateRoleTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedMandateRoleTypes.Items.ToListAsync());
    }

    /// <summary>Get a mandate role type.</summary>
    /// <response code="200">If the mandate role type is found.</response>
    /// <response code="404">If the mandate role type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.MandateRoleTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }

    /// <summary>Create a mandate role type.</summary>
    /// <response code="201">If the mandate role type is created, together with the location.</response>
    /// <response code="400">If the mandate role type information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateMandateRoleTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateMandateRoleTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update a mandate role type.</summary>
    /// <response code="200">If the mandate role type is updated, together with the location.</response>
    /// <response code="400">If the mandate role type information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateMandateRoleTypeRequest message)
    {
        var internalMessage = new UpdateMandateRoleTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateMandateRoleTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.MandateRoleTypeId });
    }
}
