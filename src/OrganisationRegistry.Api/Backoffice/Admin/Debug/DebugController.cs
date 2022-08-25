namespace OrganisationRegistry.Api.Backoffice.Admin.Debug;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using ISession = OrganisationRegistry.Infrastructure.Domain.ISession;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("debug")]
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.Developer)]
public class DebugController : OrganisationRegistryController
{
    /// <summary>
    /// Gets the aggregate state of an organisation
    /// </summary>
    /// <response code="200">If the organisation is found.</response>
    /// <response code="404">If the organisation cannot be found.</response>
    [HttpGet("organisation/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisationAggregate([FromServices] ISession session, [FromRoute] Guid id)
        => await OkAsync(session.Get<Organisation>(id));
}
