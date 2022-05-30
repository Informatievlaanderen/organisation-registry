namespace OrganisationRegistry.Api.Backoffice.Kbo;

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac.Features.OwnedInstances;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("kbo")]
public class KboController : OrganisationRegistryController
{
    private readonly ILogger<KboController> _logger;

    public KboController(
        ILogger<KboController> logger,
        ICommandSender commandSender) : base(commandSender)
    {
        _logger = logger;
    }

    /// <summary>Find organisation in KBO.</summary>
    /// <response code="200">If the organisation is found.</response>
    /// <response code="404">If the kbo number does not exist</response>
    [HttpGet("{kboNumberInput}")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromServices] Func<Owned<OrganisationRegistryContext>> contextFactory,
        [FromServices] IKboOrganisationRetriever kboOrganisationRetriever,
        [FromServices] ISecurityService securityService,
        [FromRoute] string kboNumberInput)
    {
        var kboNumber = new KboNumber(kboNumberInput);
        var dotFormat = kboNumber.ToDotFormat();
        var digitsOnly = kboNumber.ToDigitsOnly();

        await using (var organisationRegistryContext = contextFactory().Value)
        {
            if (await organisationRegistryContext
                    .OrganisationDetail
                    .AsQueryable()
                    .AnyAsync(x => x.KboNumber != null && (x.KboNumber.Equals(dotFormat) || x.KboNumber.Equals(digitsOnly))))
            {
                ModelState.AddModelError(
                    key: "Duplicate",
                    errorMessage: $"Organisatie met KBO nummer {kboNumber} is reeds gedefinieerd in wegwijs.");

                return BadRequest(ModelState);
            }
        }

        var kboOrganisationResult = await kboOrganisationRetriever.RetrieveOrganisation(await securityService.GetRequiredUser(ClaimsPrincipal.Current), kboNumber);

        if (kboOrganisationResult.HasErrors)
        {
            kboOrganisationResult.ErrorMessages
                .ToList()
                .ForEach(x => _logger.LogWarning("{Message}", x));

            return NotFound();
        }

        return Ok(kboOrganisationResult.Value);
    }
}