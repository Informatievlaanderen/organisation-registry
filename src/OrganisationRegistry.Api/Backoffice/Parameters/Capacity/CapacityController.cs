namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity;

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
[OrganisationRegistryRoute("capacities")]
public class CapacityController : OrganisationRegistryController
{
    public CapacityController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available capacities.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<CapacityListQuery.CapacityListFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedCapacities = new CapacityListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedCapacities.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedCapacities.Items.ToListAsync());
    }

    /// <summary>Get a capacity.</summary>
    /// <response code="200">If the capacity is found.</response>
    /// <response code="404">If the capacity cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.CapacityList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }

    /// <summary>Create a capacity.</summary>
    /// <response code="201">If the capacity is created, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateCapacityRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateCapacityRequestMapping.Map(message));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update a capacity.</summary>
    /// <response code="200">If the capacity is updated, together with the location.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCapacityRequest message)
    {
        var internalMessage = new UpdateCapacityInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateCapacityRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.CapacityId });
    }

    /// <summary>
    /// Remove a capacity
    /// </summary>
    /// <response code="204">If the capacity is successfully removed.</response>
    /// <response code="400">If the capacity information does not pass validation.</response>
    [HttpDelete("{id}")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var internalMessage = new RemoveCapacityRequest(id);

        await CommandSender.Send(RemoveCapacityRequestMapping.Map(internalMessage));

        return NoContent();
    }
}
