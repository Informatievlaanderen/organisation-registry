namespace OrganisationRegistry.Api.Edit.Organisation
{
    using System;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Infrastructure.Swagger;
    using Infrastructure.Swagger.Examples;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;
    using Swashbuckle.AspNetCore.Filters;
    using ForbiddenResponseExamples = Infrastructure.Swagger.Examples.ForbiddenResponseExamples;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [FeatureGate(FeatureFlags.EditApi)]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/keys")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Organisatiesleutels")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class OrganisationKeyController : OrganisationRegistryController
    {
        public OrganisationKeyController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Voeg een organisatiesleutel toe.</summary>
        /// <remarks>Voegt een organisatiesleutel toe aan een organisatie.
        /// <br />
        /// Organisatiesleutels van hetzelfde type mogen niet overlappen in tijd.</remarks>
        /// <param name="organisationId">Id van de organisatie.</param>
        /// <response code="201">Als het verzoek aanvaard is.</response>
        /// <response code="400">Als het verzoek ongeldige data bevat.<example>a;sdlf</example></response>
        /// <response code="403">Als u onvoldoende rechten heeft op dit sleuteltype.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(EmptyResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status403Forbidden, typeof(ForbiddenResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [SwaggerLocationHeader]
        public async Task<IActionResult> Post(
            [FromServices] IEditSecurityService securityService,
            [FromRoute] Guid organisationId,
            [FromBody] AddOrganisationKeyRequest message)
        {
            var internalMessage = new AddOrganisationKeyInternalRequest(organisationId, message);

            if (!securityService.CanAddKey(message.KeyTypeId))
                return Forbid();

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            var addOrganisationKey = AddOrganisationKeyRequestMapping.Map(internalMessage);
            addOrganisationKey.User = new User("Orafin", "Edit Api", "Orafin Edit Api", null, new[] { Role.Orafin });
            await CommandSender.Send(addOrganisationKey);

            return Created(
                Url.Action(nameof(Backoffice.Organisation.OrganisationKeyController.Get),
                    new { id = message.OrganisationKeyId }), null);
        }

        /// <summary>Pas een organisatiesleutel aan.</summary>
        /// <param name="organisationId">Id van de organisatie.</param>
        /// <param name="organisationKeyId">Id van de organisatiesleutel.</param>
        /// <response code="200">Indien de organisatiesleutel succesvol is aangepast.</response>
        /// <response code="400">Indien er validatiefouten zijn.</response>
        [HttpPut("{organisationKeyId:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationErrors), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EmptyResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Put(
            [FromServices] IEditSecurityService securityService,
            [FromRoute] Guid organisationKeyId,
            [FromRoute] Guid organisationId,
            [FromBody] UpdateOrganisationKeyRequest message)
        {
            var internalMessage = new UpdateOrganisationKeyInternalRequest(
                organisationId,
                organisationKeyId,
                message);

            if (!securityService.CanEditKey(message.KeyTypeId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit sleuteltype.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            var updateOrganisationKey = UpdateOrganisationKeyRequestMapping.Map(internalMessage);
            updateOrganisationKey.User = new User("Orafin", "Edit Api", "Orafin Edit Api", null, new[] { Role.Orafin });
            await CommandSender.Send(updateOrganisationKey);

            return Ok();
        }
    }
}
