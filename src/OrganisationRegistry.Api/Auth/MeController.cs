namespace OrganisationRegistry.Api.Auth;

using System;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using Responses;
using Swashbuckle.AspNetCore.Filters;
using ApiException = Be.Vlaanderen.Basisregisters.Api.Exceptions.ApiException;
using ForbiddenResponseExamples = Infrastructure.Swagger.Examples.ForbiddenResponseExamples;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("me")]
[ApiExplorerSettings(GroupName = "Authorization")]
[Consumes("application/json")]
[Produces("application/json")]
public class MeController(ISecurityService securityService) : OrganisationRegistryController
{
    /// <summary>In Flemish</summary>
    /// <remarks>In Flemish</remarks>
    /// <response code="200">.</response>
    /// <response code="401">De authenticatie is ongeldig of verlopen.</response>
    /// <response code="403">De gebruiker beschikt niet over een geldige Wegwijs-rol.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MeResponsOkExamples))]
    [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<ActionResult<MeResponse>> Get()
    {
        try
        {
            var user = await securityService.GetUser(User);

            if (user == WellknownUsers.Nobody)
                throw new ApiException("De gebruiker beschikt niet over een geldige Wegwijs-rol.", 403);

            var fullname = $"{user.FirstName} {user.LastName}".Trim();
            var roles = user.Roles.Select(i => i.ToString());

            //? TODO: Rolemapping
            var role = string.Join(';', roles);

            //! TODO: Permissions
            var permissions = Array.Empty<string>();

            return Ok(MeResponse.Create(fullname, role, permissions));
        }
        catch(ApiException) { throw; }
        catch (Exception)
        {
            //No user
            throw new ApiException("De gebruiker is niet geauthenticeerd.", 401);
        }
    }
}
