﻿namespace OrganisationRegistry.Api.Backoffice.Body.Validity;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
public class BodyValidityController : OrganisationRegistryController
{
    /// <summary>Get a body's validity.</summary>
    /// <response code="200">If the body is found.</response>
    /// <response code="404">If the body cannot be found.</response>
    [HttpGet("{id}/validity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var body = await context.BodyDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (body == null)
            return NotFound();

        return Ok(new BodyValidityResponse(body));
    }
}
