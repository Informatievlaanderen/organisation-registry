namespace OrganisationRegistry.Api.Kbo
{
    using Configuration;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using SqlServer.Infrastructure;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure.Security;
    using Magda;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("kbo")]
    public class KboController : OrganisationRegistryController
    {
        private readonly MagdaConfiguration _magdaConfiguration;
        private readonly ApiConfiguration _apiConfiguration;

        public KboController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> apiOptions,
            MagdaConfiguration magdaConfiguration) : base(commandSender)
        {
            _magdaConfiguration = magdaConfiguration;
            _apiConfiguration = apiOptions.Value;
        }

        /// <summary>Find organisation in KBO.</summary>
        /// <response code="200">If the organisation is found.</response>
        /// <response code="404">If the kbo number does not exist</response>
        [HttpGet("{kboNumberInput}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(NotFoundResult), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IDateTimeProvider dateTimeProvider,
            [FromRoute] string kboNumberInput)
        {
            var kboNumber = new KboNumber(kboNumberInput);
            var dotFormat = kboNumber.ToDotFormat();
            var digitsOnly = kboNumber.ToDigitsOnly();

            if (await context
                .OrganisationDetail
                .AnyAsync(x => x.KboNumber.Equals(dotFormat) || x.KboNumber.Equals(digitsOnly)))
            {
                ModelState.AddModelError(
                    key: "Duplicate",
                    errorMessage: $"Organisatie met KBO nummer {kboNumber} is reeds gedefinieerd in wegwijs.");

                return BadRequest(ModelState);
            }

            var kboOrganisation = await
                new KboOrganisationRetriever(_magdaConfiguration, () => context, dateTimeProvider)
                    .RetrieveOrganisation(User, kboNumber);

            if (kboOrganisation == null)
                return NotFound();

            return Ok(kboOrganisation);
        }
    }
}
