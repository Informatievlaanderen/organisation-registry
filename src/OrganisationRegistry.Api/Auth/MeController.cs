namespace OrganisationRegistry.Api.Auth;

using System;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using Responses;
using Swashbuckle.AspNetCore.Filters;
using ApiException = Be.Vlaanderen.Basisregisters.Api.Exceptions.ApiException;
using Microsoft.AspNetCore.Authorization;
using OrganisationRegistry.Api.Auth.Authorization;
using OrganisationRegistry.Handling.Authorization;

using OrganisationResource = Models.Resource<Models.OrganisationResources>;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiExplorerSettings(GroupName = "Authorization")]
[OrganisationRegistryRoute("me")]
[Consumes("application/json")]
[Produces("application/json")]
public class MeController(
    ILogger<MeController> logger,
    ISecurityService securityService
) : OrganisationRegistryController
{
    /// <summary>Gegevens van de huidige gebruiker.</summary>
    /// <remarks>Haalt de gegevens op van de gebruiker die momenteel aangemeld is.</remarks>
    /// <response code="200">De gegevens van de aangemelde gebruiker zijn opgehaald.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet]
    [OrProtected] // unchanged: /v1/me stays gated by the coarse allow-list, not the matrix
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MeResponsOkExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<ActionResult<MeResponse>> Get()
    {
        try
        {
            var user = await securityService.GetUser(User);

            if (user == WellknownUsers.Nobody)
                throw new ApiException("De gebruiker beschikt niet over een geldige Wegwijs-rol.", 403);

            var fullname = $"{user.FirstName} {user.LastName}".Trim();
            var role = user.Roles.First();

            var permissions = RolePermissions.Resolve(role);

            return Ok(MeResponse.Create(fullname, role.ToString(), permissions));
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception)
        {
            //No user
            throw new ApiException("De gebruiker is niet geauthenticeerd.", 401);
        }
    }

    [HttpGet("sample")]
    [GlobalResource(GlobalResources.RefParameters, CrudOperation.Delete)]
    public async Task<ActionResult<MeResponse>> GetSample()
    {
        var user = await securityService.GetUser(User);
        var fullname = $"{user.FirstName} {user.LastName}".Trim();
        var role = user.Roles.First();
        var permissions = RolePermissions.Resolve(role);

        return Ok(MeResponse.Create(fullname, role.ToString(), permissions));
    }

    [HttpGet("sample2")]
    [GlobalResource(GlobalResources.RefParameters, CrudOperation.Write | CrudOperation.Read)]
    public async Task<ActionResult<MeResponse>> GetSample2()
    {
        var user = await securityService.GetUser(User);
        var fullname = $"{user.FirstName} {user.LastName}".Trim();
        var role = user.Roles.First();
        var permissions = RolePermissions.Resolve(role);

        return Ok(MeResponse.Create(fullname, role.ToString(), permissions));
    }

    [HttpGet("sample3")]
    [GlobalResource(GlobalResources.RefParameters, CrudOperation.Write | CrudOperation.Read)]
    public async Task<ActionResult<MeResponse>> GetSample3(
        Guid organisationId,
        Guid capacityId,
        [FromServices] IAuthorizationService authorizationService)
    {
        //Current
        var result = await authorizationService.AuthorizeAsync(User, (ctx) =>
        {
            var policy = new CapacityPolicy(
            ctx.MemoryCaches.OvoNumbers[organisationId],
            ctx.Configuration,
            capacityId);

            return policy;
        });

        //GOALS
        // var result = await authorizationService.AuthorizeAsync(User, OrganisationResources.CanManageRegulations ,c => c)
        // result.Succeeded()

        // ignore
        var user = await securityService.GetUser(User);
        var fullname = $"{user.FirstName} {user.LastName}".Trim();
        var role = user.Roles.First();
        var permissions = RolePermissions.Resolve(role);

        return Ok(MeResponse.Create(fullname, role.ToString(), permissions));
    }
}
