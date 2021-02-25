namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Common;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationOpeningHoursSpecification :
        BaseProjection<OrganisationOpeningHoursSpecification>,
        IEventHandler<OrganisationOpeningHourAdded>,
        IEventHandler<OrganisationOpeningHourUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;

        public OrganisationOpeningHoursSpecification(
            ILogger<OrganisationOpeningHoursSpecification> logger,
            Elastic elastic)
            : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOpeningHourAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.OpeningHours == null)
                organisationDocument.OpeningHours = new List<OrganisationDocument.OrganisationOpeningHour>();
            organisationDocument.OpeningHours.RemoveExistingListItems(x => x.OrganisationOpeningHourId == message.Body.OrganisationOpeningHourId);

            organisationDocument.OpeningHours.Add(
                new OrganisationDocument.OrganisationOpeningHour(
                    message.Body.OrganisationOpeningHourId,
                    message.Body.Opens,
                    message.Body.Closes,
                    message.Body.DayOfWeek,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOpeningHourUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.OpeningHours == null)
                organisationDocument.OpeningHours = new List<OrganisationDocument.OrganisationOpeningHour>();
            organisationDocument.OpeningHours.RemoveExistingListItems(x => x.OrganisationOpeningHourId == message.Body.OrganisationOpeningHourId);

            organisationDocument.OpeningHours.Add(
                new OrganisationDocument.OrganisationOpeningHour(
                    message.Body.OrganisationOpeningHourId,
                    message.Body.Opens,
                    message.Body.Closes,
                    message.Body.DayOfWeek,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            foreach (var (key, value) in message.Body.FieldsToTerminate.OpeningHoursToTerminate)
            {
                var organisationOpeningHour =
                    organisationDocument
                        .OpeningHours
                        .Single(x => x.OrganisationOpeningHourId == key);

                organisationOpeningHour.Validity.End = value;
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
