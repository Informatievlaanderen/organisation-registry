namespace OrganisationRegistry.SqlServer.DelegationAssignments
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Person;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;

    public class DelegationAssignmentListItem
    {
        public Guid Id { get; set; }

        public Guid BodyId { get; set; }
        public Guid BodySeatId { get; set; }
        public Guid BodyMandateId { get; set; }

        public Guid PersonId { get; set; }
        public string PersonName { get; set; }

        public string ContactsJson { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class DelegationAssignmentListConfiguration : EntityMappingConfiguration<DelegationAssignmentListItem>
    {
        public const string TableName = nameof(DelegationAssignmentListView.ProjectionTables.DelegationAssignmentList);
        public const int PersonNameLength = PersonListConfiguration.NameLength;

        public override void Map(EntityTypeBuilder<DelegationAssignmentListItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.BodyId);
            b.Property(p => p.BodySeatId);
            b.Property(p => p.BodyMandateId);

            b.Property(p => p.PersonId);
            b.Property(p => p.PersonName).HasMaxLength(PersonNameLength);

            b.Property(p => p.ContactsJson);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => new { x.BodyMandateId, x.PersonName }).IsClustered();
            b.HasIndex(x => x.BodyMandateId);
            b.HasIndex(x => x.PersonName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class DelegationAssignmentListView :
        Projection<DelegationAssignmentListView>,
        IEventHandler<PersonAssignedToDelegation>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<PersonAssignedToDelegationRemoved>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            DelegationAssignmentList
        }

        private readonly IEventStore _eventStore;

        public DelegationAssignmentListView(
            ILogger<DelegationAssignmentListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public DelegationAssignmentListView(
            ILogger<DelegationAssignmentListView> logger,
            IContextFactory contextFactory) : base(logger, contextFactory) { }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            var delegationAssignment = new DelegationAssignmentListItem
            {
                Id = message.Body.DelegationAssignmentId,
                BodyId = message.Body.BodyId,
                BodySeatId = message.Body.BodySeatId,
                BodyMandateId = message.Body.BodyMandateId,
                PersonId = message.Body.PersonId,
                PersonName = message.Body.PersonFullName,
                ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>()),
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo,

            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.DelegationAssignmentList.Add(delegationAssignment);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var delegationAssignment =
                    context.DelegationAssignmentList.Single(item => item.Id == message.Body.DelegationAssignmentId);

                delegationAssignment.PersonId = message.Body.PersonId;
                delegationAssignment.PersonName = message.Body.PersonFullName;
                delegationAssignment.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>());
                delegationAssignment.ValidFrom = message.Body.ValidFrom;
                delegationAssignment.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var delegationAssignment =
                    context.DelegationAssignmentList.Single(item => item.Id == message.Body.DelegationAssignmentId);

                context.Remove(delegationAssignment);
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
