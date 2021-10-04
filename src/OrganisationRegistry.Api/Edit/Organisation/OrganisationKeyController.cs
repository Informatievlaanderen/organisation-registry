namespace OrganisationRegistry.Api.Edit.Organisation
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;

    [FeatureGate(FeatureFlags.EditApi)]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/keys")]
    public class OrganisationKeyController : OrganisationRegistryController
    {
        public OrganisationKeyController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Create a key for an organisation.</summary>
        /// <response code="201">If the key is created, together with the location.</response>
        /// <response code="400">If the key information does not pass validation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
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

        /// <summary>Update a key for an organisation.</summary>
        /// <response code="201">If the key is updated, together with the location.</response>
        /// <response code="400">If the key information does not pass validation.</response>
        [HttpPut("{organisationKeyId:guid}")]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
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
