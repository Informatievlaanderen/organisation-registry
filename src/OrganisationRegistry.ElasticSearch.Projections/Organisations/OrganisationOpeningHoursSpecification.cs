namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using ElasticSearch.Organisations;
    using Infrastructure.Change;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationOpeningHoursSpecification :
        BaseProjection<OrganisationOpeningHoursSpecification>,
        IElasticEventHandler<OrganisationOpeningHourAdded>,
        IElasticEventHandler<OrganisationOpeningHourUpdated>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {
        public OrganisationOpeningHoursSpecification(
            ILogger<OrganisationOpeningHoursSpecification> logger)
            : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOpeningHourAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.OpeningHours == null)
                        document.OpeningHours = new List<OrganisationDocument.OrganisationOpeningHour>();
                    document.OpeningHours.RemoveExistingListItems(x => x.OrganisationOpeningHourId == message.Body.OrganisationOpeningHourId);

                    document.OpeningHours.Add(
                        new OrganisationDocument.OrganisationOpeningHour(
                            message.Body.OrganisationOpeningHourId,
                            message.Body.Opens,
                            message.Body.Closes,
                            message.Body.DayOfWeek,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOpeningHourUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.OpeningHours == null)
                        document.OpeningHours = new List<OrganisationDocument.OrganisationOpeningHour>();
                    document.OpeningHours.RemoveExistingListItems(x => x.OrganisationOpeningHourId == message.Body.OrganisationOpeningHourId);

                    document.OpeningHours.Add(
                        new OrganisationDocument.OrganisationOpeningHour(
                            message.Body.OrganisationOpeningHourId,
                            message.Body.Opens,
                            message.Body.Closes,
                            message.Body.DayOfWeek,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));

                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.OpeningHours)
                    {
                        var organisationOpeningHour =
                            document
                                .OpeningHours
                                .Single(x => x.OrganisationOpeningHourId == key);

                        organisationOpeningHour.Validity.End = value;
                    }
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.OpeningHours)
                    {
                        var organisationOpeningHour =
                            document
                                .OpeningHours
                                .Single(x => x.OrganisationOpeningHourId == key);

                        organisationOpeningHour.Validity.End = value;
                    }
                }
            );
        }
    }
}
