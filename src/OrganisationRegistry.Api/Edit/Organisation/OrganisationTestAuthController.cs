namespace OrganisationRegistry.Api.Edit.Organisation
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;
    using AuthenticationSchemes = Infrastructure.Security.AuthenticationSchemes;

    [FeatureGate(FeatureFlags.EditApi)]
    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("edit")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Test Authenticatie")]
    public class OrganisationTestAuthController : OrganisationRegistryController
    {
        public OrganisationTestAuthController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>
        /// Test de authenticatie.
        /// </summary>
        /// <remarks>Tijdelijk endpoint bedoeld om de authenticatie te testen.</remarks>
        /// <returns></returns>
        [HttpGet("auth")]
        [Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi)]
        public async Task<IActionResult> TestAuthorization()
        {
            return Ok();
        }
    }
}
