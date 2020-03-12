using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganisationRegistry.Api.Kbo
{
    using System.Security.Claims;
    using Configuration;
    using Microsoft.AspNetCore.Hosting;
    using Responses;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationRetriever: IKboOrganisationRetriever
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGeefOndernemingQuery _geefOndernemingQuery;

        public KboOrganisationRetriever(
            IDateTimeProvider dateTimeProvider,
            IGeefOndernemingQuery geefOndernemingQuery)
        {
            _dateTimeProvider = dateTimeProvider;
            _geefOndernemingQuery = geefOndernemingQuery;
        }

        public async Task<IMagdaOrganisationResponse> RetrieveOrganisation(ClaimsPrincipal user, KboNumber kboNumber)
        {
            var kboOrganisation = await _geefOndernemingQuery.Execute(user, kboNumber.ToDigitsOnly());

            if (kboOrganisation == null)
                return null;

            var response = kboOrganisation.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (response == null || response.Uitzonderingen != null && response.Uitzonderingen.Any())
                return null;

            return new MagdaOrganisationResponse(response.Inhoud?.Onderneming, _dateTimeProvider);
        }
    }
}
