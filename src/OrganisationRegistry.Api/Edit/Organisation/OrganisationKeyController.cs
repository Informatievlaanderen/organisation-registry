namespace OrganisationRegistry.Api.Edit.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Infrastructure;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using EmptyResult = Infrastructure.EmptyResult;

    [FeatureGate(FeatureFlags.EditApi)]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/keys")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Organisatiesleutels")]
    public class OrganisationKeyController : OrganisationRegistryController
    {
        public OrganisationKeyController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Maak een organisatiesleutel aan.</summary>
        /// <param name="organisationId">Id van de organisatie.</param>
        /// <response code="201">Als het verzoek aanvaard is.</response>
        /// <response code="400">Als het verzoek ongeldige data bevat.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IActionResult> Post(
            [FromServices] IEditSecurityService securityService,
            [FromRoute] Guid organisationId,
            [FromBody] AddOrganisationKeyRequest message)
        {
            var internalMessage = new AddOrganisationKeyInternalRequest(organisationId, message);

            if (!securityService.CanAddKey(message.KeyTypeId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit sleuteltype.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            var addOrganisationKey = AddOrganisationKeyRequestMapping.Map(internalMessage);
            addOrganisationKey.User = new User("Orafin", "Edit Api", "Orafin Edit Api", null, new[] { Role.Orafin });
            await CommandSender.Send(addOrganisationKey);

            return Created(Url.Action(nameof(Backoffice.Organisation.OrganisationKeyController.Get), new { id = message.OrganisationKeyId }), null);
        }

        /// <summary>Pas een organisatiesleutel aan.</summary>
        /// <param name="organisationId">Id van de organisatie.</param>
        /// <param name="organisationKeyId">Id van de organisatiesleutel.</param>
        /// <response code="200">Indien de organisatiesleutel succesvol is aangepast.</response>
        /// <response code="400">Indien er validatiefouten zijn.</response>
        [HttpPut("{organisationKeyId:guid}")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BadRequestResponseExamples))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestResponseExamples))]
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
