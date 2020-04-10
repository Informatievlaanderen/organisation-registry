using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrganisationRegistry.Api.Kbo
{
    using System;
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

        public async Task<Result<IMagdaOrganisationResponse>> RetrieveOrganisation(ClaimsPrincipal user,
            KboNumber kboNumber)
        {
            var kboOrganisation = await _geefOndernemingQuery.Execute(user, kboNumber.ToDigitsOnly());

            if (kboOrganisation == null)
                throw new Exception("Geen antwoord van kbo gekregen.");

            var response = kboOrganisation.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (response == null)
                throw new Exception("Geen antwoord van kbo gekregen.");

            if (response.Uitzonderingen != null && response.Uitzonderingen.Any())
                return Result<IMagdaOrganisationResponse>.Fail(
                    response.Uitzonderingen.Select(type => type.Diagnose).ToArray());

            return Result<IMagdaOrganisationResponse>.Success(
                new MagdaOrganisationResponse(response.Inhoud?.Onderneming, _dateTimeProvider));
        }
    }
}
