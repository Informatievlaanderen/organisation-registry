using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganisationRegistry.Api.Kbo
{
    using System.Security.Claims;
    using Configuration;
    using Magda;
    using Microsoft.AspNetCore.Hosting;
    using Responses;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationRetriever: IKboOrganisationRetriever
    {
        private readonly MagdaConfiguration _configuration;
        private readonly Func<OrganisationRegistryContext> _contextFactory;

        public KboOrganisationRetriever(
            MagdaConfiguration configuration,
            Func<OrganisationRegistryContext> contextFactory)
        {
            _configuration = configuration;
            _contextFactory = contextFactory;
        }

        public async Task<IMagdaOrganisationResponse> RetrieveOrganisation(ClaimsPrincipal user, KboNumber kboNumber)
        {
            var kboOrganisation =
                await new GeefOndernemingQuery(_configuration, _contextFactory).Execute(user,
                    kboNumber.ToDigitsOnly());

            if (kboOrganisation == null)
                return null;

            var response = kboOrganisation.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (response == null || response.Uitzonderingen != null && response.Uitzonderingen.Any())
                return null;

            return new MagdaOrganisationResponse(response.Inhoud?.Onderneming);
        }
    }
}
