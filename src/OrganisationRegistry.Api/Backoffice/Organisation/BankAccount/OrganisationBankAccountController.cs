namespace OrganisationRegistry.Api.Backoffice.Organisation.BankAccount;

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
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/bankAccounts")]
[OrganisationRegistryAuthorize]
public class OrganisationBankAccountController : OrganisationRegistryController
{
    public OrganisationBankAccountController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available bankAccounts for an organisation.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationBankAccountListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisations = new OrganisationBankAccountListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a bankAccount for an organisation.</summary>
    /// <response code="200">If the bankAccount is found.</response>
    /// <response code="404">If the bankAccount cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationBankAccountList.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationBankAccountId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
