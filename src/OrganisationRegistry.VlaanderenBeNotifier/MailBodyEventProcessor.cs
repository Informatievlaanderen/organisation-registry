namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Body.Events;
    using Configuration;
    using Infrastructure.AppSpecific;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Extensions.Options;
    using SendGrid;

    public class MailBodyEventProcessor :
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyFormalFrameworkAdded>,
        IEventHandler<BodyFormalFrameworkUpdated>,
        IEventHandler<BodyBalancedParticipationChanged>
    {
        private readonly IMemoryCaches _memoryCaches;
        private readonly IMailer _mailer;
        private readonly VlaanderenBeNotifierConfiguration _configuration;
        private readonly TogglesConfiguration _toggles;
        private readonly string _bodyUriTemplate;
        private readonly string _bodyFormalFrameworkUriTemplate;

        public MailBodyEventProcessor(
            IMemoryCaches memoryCaches,
            IMailer mailer,
            IOptions<TogglesConfiguration> togglesOptions,
            IOptions<VlaanderenBeNotifierConfiguration> vlaanderenBeNotifierOptions)
        {
            _memoryCaches = memoryCaches;
            _mailer = mailer;
            _toggles = togglesOptions.Value;
            _configuration = vlaanderenBeNotifierOptions.Value;
            _bodyUriTemplate = _configuration.BodyUriTemplate;
            _bodyFormalFrameworkUriTemplate = _configuration.BodyFormalFrameworkUriTemplate;
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodyRegistered> message)
        {
            var subject = $"OrganisationRegistry: ORGAAN TOEGEVOEGD {message.Body.BodyNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("Er werd onlangs een nieuwe orgaan toegevoegd:")
                    .AppendLine($"{message.Body.BodyNumber} - {message.Body.Name} werd toegevoegd op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Formele geldigheidsduur: {new Period(new ValidFrom(message.Body.FormalValidFrom), new ValidTo(message.Body.FormalValidTo))}")
                    .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                    .AppendLine(string.Format(_bodyUriTemplate, message.Body.BodyId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodyAssignedToOrganisation> message)
        {
            var subject = $"OrganisationRegistry: ORGAAN {_memoryCaches.BodyNames[message.Body.BodyId]} GEKOPPELD AAN ORGANISATIE {_memoryCaches.OvoNumbers[message.Body.OrganisationId]}";
            var body =
                new StringBuilder()
                    .AppendLine("Er werd onlangs een orgaan gekoppeld aan een organisatie:")
                    .AppendLine($"Orgaan {_memoryCaches.BodyNames[message.Body.BodyId]} werd gekoppeld aan organisatie {_memoryCaches.OvoNumbers[message.Body.OrganisationId]} - {message.Body.OrganisationName} op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                    .AppendLine(string.Format(_bodyUriTemplate, message.Body.BodyId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkAdded> message)
        {
            if (_configuration.MepFormalFrameworkId != message.Body.FormalFrameworkId)
                return;

            var subject = $"OrganisationRegistry: TOEPASSINGSGEBIED MEP GEKOPPELD AAN ORGAAN {_memoryCaches.BodyNames[message.Body.BodyId]}";
            var body =
                new StringBuilder()
                    .AppendLine("Het MEP toepassingsgebied werd onlangs orgaan gekoppeld aan een orgaan:")
                    .AppendLine($"Toepassingsgebied MEP werd gekoppeld aan orgaan {_memoryCaches.BodyNames[message.Body.BodyId]} op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Geldigheidsduur: {new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo))}")
                    .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                    .AppendLine(string.Format(_bodyFormalFrameworkUriTemplate, message.Body.BodyId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalFrameworkUpdated> message)
        {
            if (_configuration.MepFormalFrameworkId != message.Body.FormalFrameworkId)
                return;

            if (message.Body.FormalFrameworkId != message.Body.PreviousFormalFrameworkId)
            {
                var subject = $"OrganisationRegistry: TOEPASSINGSGEBIED MEP GEKOPPELD AAN ORGAAN {_memoryCaches.BodyNames[message.Body.BodyId]}";
                var body =
                    new StringBuilder()
                        .AppendLine("Het MEP toepassingsgebied werd onlangs gekoppeld aan een orgaan:")
                        .AppendLine($"Toepassingsgebied MEP werd gekoppeld aan orgaan {_memoryCaches.BodyNames[message.Body.BodyId]} op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                        .AppendLine($"Geldigheidsduur: {new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo))}")
                        .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                        .AppendLine(string.Format(_bodyFormalFrameworkUriTemplate, message.Body.BodyId))
                        .ToString();

                SendMails(new Mail(subject, body));
            }
            else
            {
                var current = new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo));
                var previous = new Period(new ValidFrom(message.Body.PreviouslyValidFrom), new ValidTo(message.Body.PreviouslyValidTo));

                if (current.Equals(previous)) return;

                var subject = $"OrganisationRegistry: GELDIGHEID TOEPASSINGSGEBIED MEP GEWIJZIGD VOOR ORGAAN {_memoryCaches.BodyNames[message.Body.BodyId]}";
                var body =
                    new StringBuilder()
                        .AppendLine("De geldigheid van het MEP toepassingsgebied werd onlangs gewijzigd voor een orgaan:")
                        .AppendLine($"Geldigheid toepassingsgebied MEP werd gewijzigd voor orgaan {_memoryCaches.BodyNames[message.Body.BodyId]} op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                        .AppendLine($"Geldigheidsduur: {new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo))}")
                        .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                        .AppendLine(string.Format(_bodyFormalFrameworkUriTemplate, message.Body.BodyId))
                        .ToString();

                SendMails(new Mail(subject, body));
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyBalancedParticipationChanged> message)
        {
            if (message.Body.BalancedParticipationObligatory != message.Body.PreviousBalancedParticipationObligatory)
                return;

            var bodyName = _memoryCaches.BodyNames[message.Body.BodyId];

            var subject = $"OrganisationRegistry: MEP-PLICHTIGHEID ORGAAN '{bodyName}' GEWIJZIGD";
            var body =
                new StringBuilder()
                    .AppendLine("De mep-plichtigheid van een orgaan werd gewijzigd:")
                    .AppendLine($"{bodyName} werd aangepast op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Vorige waarde: {message.Body.PreviousBalancedParticipationObligatory}")
                    .AppendLine($"Nieuwe waarde: {message.Body.BalancedParticipationObligatory}")
                    .AppendLine($"Uitvoerder: {message.FirstName} {message.LastName}")
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        private void SendMails(params Mail[] mails)
        {
            if (!_toggles.SendVlaanderenBeNotifierMails)
                return;

            mails?
                .ToList()
                .ForEach(mail =>
                    _mailer.SendEmailAsync(fromAddress: _configuration.FromAddress,
                            fromName: _configuration.FromName,
                            to: _configuration.BodyTo,
                            subject: mail.Subject,
                            body: mail.Body,
                            categories: new List<string> { "OrganisationRegistry VlaanderenBeNotifier" }
                        )
                        .Wait());
        }
    }
}
