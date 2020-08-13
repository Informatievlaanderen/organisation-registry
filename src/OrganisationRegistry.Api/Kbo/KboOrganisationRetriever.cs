using System.Linq;
using System.Threading.Tasks;

namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using GeefOnderneming = global::Magda.GeefOnderneming;
    using RegistreerInschrijving = global::Magda.RegistreerInschrijving;
    using Microsoft.Extensions.Logging;
    using Responses;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationRetriever: IKboOrganisationRetriever
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGeefOndernemingQuery _geefOndernemingQuery;
        private readonly IRegistreerInschrijvingCommand _registerInscriptionCommand;
        private readonly ILogger<KboOrganisationRetriever> _logger;

        public KboOrganisationRetriever(
            IDateTimeProvider dateTimeProvider,
            IGeefOndernemingQuery geefOndernemingQuery,
            IRegistreerInschrijvingCommand registerInscriptionCommand,
            ILogger<KboOrganisationRetriever> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _geefOndernemingQuery = geefOndernemingQuery;
            _registerInscriptionCommand = registerInscriptionCommand;
            _logger = logger;
        }

        public async Task<Result<IMagdaOrganisationResponse>> RetrieveOrganisation(
            ClaimsPrincipal user,
            KboNumber kboNumber)
        {
            var kboNumberDotLess = kboNumber.ToDigitsOnly();

            var registerInscription = await _registerInscriptionCommand.Execute(user, kboNumberDotLess);
            var giveOrganisation = await _geefOndernemingQuery.Execute(user, kboNumberDotLess);

            var registerInscriptionReply = registerInscription?.Body?.RegistreerInschrijvingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (registerInscriptionReply == null)
                throw new Exception("Geen antwoord van magda gekregen.");

            LogExceptions(registerInscriptionReply);
            var errors = registerInscriptionReply.Uitzonderingen?
                .Where(type => type.Type == RegistreerInschrijving.UitzonderingTypeType.FOUT)
                .ToList() ?? new List<RegistreerInschrijving.UitzonderingType>();

            if (errors.Any())
                throw new Exception(
                    $"Er is een fout opgetreden tijdens het inschrijven bij magda:\n" +
                    $"{string.Join('\n', errors.Select(type => type.Diagnose))}");

            var giveOrganisationReply = giveOrganisation.Body?.GeefOndernemingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (giveOrganisationReply == null)
                throw new Exception("Geen antwoord van magda gekregen.");

            LogExceptions(giveOrganisationReply);

            if (giveOrganisationReply.Uitzonderingen != null &&
                giveOrganisationReply.Uitzonderingen.Any())
                return Result<IMagdaOrganisationResponse>.Fail(
                    giveOrganisationReply.Uitzonderingen.Select(type => type.Diagnose).ToArray());

            return Result<IMagdaOrganisationResponse>.Success(
                new MagdaOrganisationResponse(giveOrganisationReply.Inhoud?.Onderneming, _dateTimeProvider));
        }

        private void LogExceptions(RegistreerInschrijving.AntwoordType reply)
        {
            reply.Uitzonderingen?
                .Where(type => type.Type == RegistreerInschrijving.UitzonderingTypeType.INFORMATIE)
                .ToList()
                .ForEach(type => _logger.LogInformation($"{type.Diagnose}"));

            reply.Uitzonderingen?
                .Where(type => type.Type == RegistreerInschrijving.UitzonderingTypeType.WAARSCHUWING)
                .ToList()
                .ForEach(type => _logger.LogWarning($"{type.Diagnose}"));

            reply.Uitzonderingen?
                 .Where(type => type.Type == RegistreerInschrijving.UitzonderingTypeType.FOUT)
                 .ToList()
                 .ForEach(type => _logger.LogError($"{type.Diagnose}"));
        }

        private void LogExceptions(GeefOnderneming.AntwoordType reply)
        {
            reply.Uitzonderingen?
                .Where(type => type.Type == GeefOnderneming.UitzonderingTypeType.INFORMATIE)
                .ToList()
                .ForEach(type => _logger.LogInformation($"{type.Diagnose}"));

            reply.Uitzonderingen?
                .Where(type => type.Type == GeefOnderneming.UitzonderingTypeType.WAARSCHUWING)
                .ToList()
                .ForEach(type => _logger.LogWarning($"{type.Diagnose}"));

            reply.Uitzonderingen?
                .Where(type => type.Type == GeefOnderneming.UitzonderingTypeType.FOUT)
                .ToList()
                .ForEach(type => _logger.LogError($"{type.Diagnose}"));
        }
    }
}
