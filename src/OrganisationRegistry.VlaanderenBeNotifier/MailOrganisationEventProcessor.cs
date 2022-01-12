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
    using Schema;
    using SendGrid;
    using SqlServer.Infrastructure;

    public class MailOrganisationEventProcessor :
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationNameUpdated>,
        IEventHandler<OrganisationShowOnVlaamseOverheidSitesUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
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

        public async Task Handle(DbConnection connection, DbTransaction transaction, IEnvelope<OrganisationCreated> message)
        {
            var organisation = new OrganisationCacheItem
            {
                Id = message.Body.OrganisationId,
                Name = message.Body.Name,
                OvoNumber = message.Body.OvoNumber
            };

            await using var ctx = new VlaanderenBeNotifierTransactionalContext(connection, transaction);
            await ctx.OrganisationCache.AddAsync(organisation);
            await ctx.SaveChangesAsync();

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
                mails.Add(OrganisationShowOnVlaanderenBeChanged(message.Body.OvoNumber, message.Body.Name, message.Body.ShowOnVlaamseOverheidSites, message.Body.Timestamp, message.Body.OrganisationId));

            SendMails(mails.ToArray());
        }

        public async Task Handle(DbConnection connection, DbTransaction transaction, IEnvelope<OrganisationNameUpdated> message)
        {
            await using var ctx = new VlaanderenBeNotifierTransactionalContext(connection, transaction);
            var organisation = await ctx.OrganisationCache.FindAsync(message.Body.OrganisationId);

            SendMails(OrganisationNameChanged(
                message.Body.OrganisationId,
                _memoryCaches.OvoNumbers[message.Body.OrganisationId],
                message.Body.Name,
                organisation.Name,
                message.Body.Timestamp));

            organisation.Name = message.Body.Name;

            throw new NotImplementedException();
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationShowOnVlaamseOverheidSitesUpdated> message)
        {
            SendMails(
                OrganisationShowOnVlaanderenBeChanged(
                    _memoryCaches.OvoNumbers[message.Body.OrganisationId],
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    message.Body.ShowOnVlaamseOverheidSites,
                    message.Body.Timestamp,
                    message.Body.OrganisationId)
            );
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

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            var mails = new List<Mail>();

            var organisationHasChangedName = message.Body.NameFromKboCoupling != message.Body.NameBeforeKboCoupling;

            if (organisationHasChangedName)
                mails.Add(OrganisationNameChanged(
                    message.Body.OrganisationId,
                    message.Body.OvoNumber,
                    message.Body.NameBeforeKboCoupling,
                    message.Body.NameFromKboCoupling,
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

        private Mail OrganisationShowOnVlaanderenBeChanged(string ovoNumber, string organisationName, bool showOnVlaamseOverheidSites, DateTimeOffset timestamp, Guid organisationId)
        {
            var subject = $"OrganisationRegistry: TONEN IN WEBSITES VLAAMSE OVERHEID AANGEPAST VOOR ORGANISATIE {ovoNumber}";
            var body =
                new StringBuilder()
                    .AppendLine("De volgende organisatie heeft de optie \'Tonen in websites Vlaamse Overheid\' aangepast")
                    .AppendLine($"{ovoNumber} - {organisationName} werd aangepast op {timestamp:yy-MM-dd HH:mm:ss}.")
                    .AppendLine($"Nieuwe waarde: {showOnVlaamseOverheidSites}")
                    .AppendLine(string.Format(_organisationUriTemplate, organisationId))
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
        }
    }
}
