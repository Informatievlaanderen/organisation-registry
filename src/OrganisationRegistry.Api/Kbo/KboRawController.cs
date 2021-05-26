namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using global::Magda.RegistreerInschrijving;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("kboraw")]
    public class KboRawController : OrganisationRegistryController
    {
        private readonly ILogger<KboRawController> _logger;
        private readonly IRegistreerInschrijvingCommand _registerInscriptionCommand;
        private readonly IGeefOndernemingQuery _geefOndernemingQuery;

        public KboRawController(
            ILogger<KboRawController> logger,
            IRegistreerInschrijvingCommand registerInscriptionCommand,
            IGeefOndernemingQuery geefOndernemingQuery,
            ICommandSender commandSender) : base(commandSender)
        {
            _logger = logger;
            _registerInscriptionCommand = registerInscriptionCommand;
            _geefOndernemingQuery = geefOndernemingQuery;
        }

        /// <summary>Return raw result from magda kbo lookup.</summary>
        /// <response code="200">The raw request/response from magda kbo lookup.</response>
        [HttpGet("{kboNumberInput}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.Developer)]
        [ProducesResponseType(typeof(NotFoundResult), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(
            [FromServices] ISecurityService securityService,
            [FromRoute] string kboNumberInput)
        {
            var kboNumber = new KboNumber(kboNumberInput);
            var digitsOnly = kboNumber.ToDigitsOnly();

            var user = securityService.GetUser(User);
            var registerInscription = await _registerInscriptionCommand.Execute(user, digitsOnly);
            var giveOrganisation = await _geefOndernemingQuery.Execute(user, digitsOnly);

            return Ok(JsonConvert.SerializeObject(new
            {
                registerInscription,
                giveOrganisation
            }));
        }
    }
}
