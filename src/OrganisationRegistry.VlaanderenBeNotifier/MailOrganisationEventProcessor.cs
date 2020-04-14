namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Infrastructure.AppSpecific;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Extensions.Options;
    using Organisation.Events;
    using SendGrid;

    public class MailOrganisationEventProcessor :
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationBecameActive>,
        IEventHandler<OrganisationBecameInactive>
    {
        private readonly IMemoryCaches _memoryCaches;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMailer _mailer;
        private readonly VlaanderenBeNotifierConfiguration _configuration;
        private readonly TogglesConfiguration _toggles;
        private readonly string _organisationUriTemplate;

        public MailOrganisationEventProcessor(
            IMemoryCaches memoryCaches,
            IDateTimeProvider dateTimeProvider,
            IMailer mailer,
            IOptions<TogglesConfiguration> togglesOptions,
            IOptions<VlaanderenBeNotifierConfiguration> vlaanderenBeNotifierOptions)
        {
            _memoryCaches = memoryCaches;
            _dateTimeProvider = dateTimeProvider;
            _mailer = mailer;
            _toggles = togglesOptions.Value;
            _configuration = vlaanderenBeNotifierOptions.Value;
            _organisationUriTemplate = _configuration.OrganisationUriTemplate;
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationCreated> message)
        {
            var subject = $"OrganisationRegistry: ORGANISATIE TOEGEVOEGD {message.Body.OvoNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("Er werd onlangs een nieuwe organisatie toegevoegd:")
                    .AppendLine($"{message.Body.OvoNumber} - {message.Body.Name} werd toegevoegd op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Actief: {new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo)).OverlapsWith(_dateTimeProvider.Today)}")
                    .AppendLine($"Tonen op sites Vlaamse Overheid: {message.Body.ShowOnVlaamseOverheidSites}")
                    .AppendLine($"Tonen op sites Vlaamse Overheid: {message.Body.ShowOnVlaamseOverheidSites}")
                    .AppendLine(string.Format(_organisationUriTemplate, message.Body.OrganisationId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            var subject = $"OrganisationRegistry: ORGANISATIE TOEGEVOEGD {message.Body.OvoNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("Er werd onlangs een nieuwe organisatie toegevoegd:")
                    .AppendLine($"{message.Body.OvoNumber} - {message.Body.Name} werd toegevoegd op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Actief: {new Period(new ValidFrom(message.Body.ValidFrom), new ValidTo(message.Body.ValidTo)).OverlapsWith(_dateTimeProvider.Today)}")
                    .AppendLine($"Tonen op sites Vlaamse Overheid: {message.Body.ShowOnVlaamseOverheidSites}")
                    .AppendLine($"Tonen op sites Vlaamse Overheid: {message.Body.ShowOnVlaamseOverheidSites}")
                    .AppendLine(string.Format(_organisationUriTemplate, message.Body.OrganisationId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationBecameActive> message)
        {
            var subject = $"OrganisationRegistry: ORGANISATIE WERD ACTIEF {_memoryCaches.OvoNumbers[message.Body.OrganisationId]}";
            var body =
                new StringBuilder()
                    .AppendLine("Een organisatie is onlangs ACTIEF geworden:")
                    .AppendLine($"{_memoryCaches.OvoNumbers[message.Body.OrganisationId]} - {_memoryCaches.OrganisationNames[message.Body.OrganisationId]} werd aangepast op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine(string.Format(_organisationUriTemplate, message.Body.OrganisationId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationBecameInactive> message)
        {
            var subject = $"OrganisationRegistry: ORGANISATIE WERD INACTIEF {_memoryCaches.OvoNumbers[message.Body.OrganisationId]}";
            var body =
                new StringBuilder()
                    .AppendLine("Een organisatie is onlangs INACTIEF geworden:")
                    .AppendLine($"{_memoryCaches.OvoNumbers[message.Body.OrganisationId]} - {_memoryCaches.OrganisationNames[message.Body.OrganisationId]} werd aangepast op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine(string.Format(_organisationUriTemplate, message.Body.OrganisationId))
                    .ToString();

            SendMails(new Mail(subject, body));
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationInfoUpdated> message)
        {
            var mails = new List<Mail>();

            var organisationHasChangedName = message.Body.Name != message.Body.PreviousName;
            var organisationHasChangedShowOnVlaanderenBe = message.Body.ShowOnVlaamseOverheidSites != message.Body.PreviouslyShownInVlaanderenBe;

            if (organisationHasChangedName)
                mails.Add(OrganisationNameChanged(
                    message.Body.OrganisationId,
                    message.Body.OvoNumber,
                    message.Body.Name,
                    message.Body.PreviousName,
                    message.Body.Timestamp));

            if (organisationHasChangedShowOnVlaanderenBe)
                mails.Add(OrganisationShowOnVlaanderenBeChanged(message));

            SendMails(mails.ToArray());
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            var mails = new List<Mail>();

            var organisationHasChangedName = message.Body.Name != message.Body.PreviousName;

            if (organisationHasChangedName)
                mails.Add(OrganisationNameChanged(
                    message.Body.OrganisationId,
                    message.Body.OvoNumber,
                    message.Body.Name,
                    message.Body.PreviousName,
                    message.Body.Timestamp));

            SendMails(mails.ToArray());
        }

        private Mail OrganisationNameChanged(Guid organisationId, string ovoNumber, string name, string previousName,
            DateTimeOffset messageTimestamp)
        {
            var subject = $"OrganisationRegistry: NAAMSVERANDERING ORGANISATIE {ovoNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("Een organisatie is onlangs veranderd van naam:")
                    .AppendLine($"{ovoNumber} - {name} werd aangepast op {messageTimestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Oude naam: {previousName}")
                    .AppendLine(string.Format(_organisationUriTemplate, organisationId))
                    .ToString();

            return new Mail(subject, body);
        }

        private Mail OrganisationShowOnVlaanderenBeChanged(IEnvelope<OrganisationInfoUpdated> message)
        {
            var subject = $"OrganisationRegistry: TONEN IN WEBSITES VLAAMSE OVERHEID AANGEPAST VOOR ORGANISATIE {message.Body.OvoNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("De volgende organisatie heeft de optie \'Tonen in websites Vlaamse Overheid\' aangepast")
                    .AppendLine($"{message.Body.OvoNumber} - {message.Body.Name} werd aangepast op {message.Body.Timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Vorige waarde: {message.Body.PreviouslyShownInVlaanderenBe}")
                    .AppendLine($"Nieuwe waarde: {message.Body.ShowOnVlaamseOverheidSites}")
                    .AppendLine(string.Format(_organisationUriTemplate, message.Body.OrganisationId))
                    .ToString();

            return new Mail(subject, body);
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
                            to: _configuration.OrganisationTo,
                            subject: mail.Subject,
                            body: mail.Body,
                            categories: new List<string>() { "OrganisationRegistry VlaanderenBeNotifier" }
                        )
                        .Wait());
            //_telemetryClient.TrackEvent("VlaanderenBeNotifier::MailSent");
            //_telemetryClient.TrackEvent("VlaanderenBeNotifier::MailSent::Organisation");
        }
    }
}
