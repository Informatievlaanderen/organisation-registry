namespace OrganisationRegistry.SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class ActivePeopleAssignedToBodyMandateListItem
    {
        public Guid DelegationAssignmentId { get; set; }

        public Guid PersonId { get; set; }

        public string PersonFullName { get; set; }

        public Guid BodySeatId { get; set; }

        public Guid BodyMandateId { get; set; }

        public Guid BodyId { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class ActivePeopleAssignedToBodyMandatesListConfiguration : EntityMappingConfiguration<ActivePeopleAssignedToBodyMandateListItem>
    {
        public override void Map(EntityTypeBuilder<ActivePeopleAssignedToBodyMandateListItem> b)
        {
            b.ToTable(nameof(ActivePeopleAssignedToBodyMandatesListView.ProjectionTables.ActivePeopleAssignedToBodyMandatesList), "OrganisationRegistry")
                .HasKey(p => p.DelegationAssignmentId)
                .IsClustered(false);

            b.Property(p => p.PersonId).IsRequired();

            b.Property(p => p.PersonFullName).IsRequired();

            b.Property(p => p.BodySeatId).IsRequired();

            b.Property(p => p.BodyMandateId).IsRequired();

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ValidTo);
        }
    }

    public class ActivePeopleAssignedToBodyMandatesListView :
        Projection<ActivePeopleAssignedToBodyMandatesListView>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<AssignedPersonAssignedToBodyMandate>,
        IEventHandler<AssignedPersonClearedFromBodyMandate>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ActivePeopleAssignedToBodyMandatesListView(
            ILogger<ActivePeopleAssignedToBodyMandatesListView> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider
        ) : base(logger)
        {
            _contextFactory = contextFactory;
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ActivePeopleAssignedToBodyMandatesList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            var validTo = new ValidTo(message.Body.ValidTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activePerson =
                    context.ActivePeopleAssignedToBodyMandatesList
                        .SingleOrDefault(item => item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                if (activePerson == null)
                    return;

                activePerson.PersonId = message.Body.PersonId;
                activePerson.PersonFullName = message.Body.PersonFullName;
                activePerson.ValidTo = validTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonAssignedToBodyMandate> message)
        {
            var validTo = new ValidTo(message.Body.ValidTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            var activePersonListItem = new ActivePeopleAssignedToBodyMandateListItem
            {
                DelegationAssignmentId = message.Body.DelegationAssignmentId,
                BodySeatId = message.Body.BodySeatId,
                BodyMandateId = message.Body.BodyMandateId,
                BodyId = message.Body.BodyId,
                PersonId = message.Body.PersonId,
                PersonFullName = message.Body.PersonFullName,
                ValidTo = validTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.ActivePeopleAssignedToBodyMandatesList.Add(activePersonListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonClearedFromBodyMandate> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activePersonListItem =
                    context.ActivePeopleAssignedToBodyMandatesList
                        .SingleOrDefault(item =>
                            item.BodyId == message.Body.BodyId &&
                            item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                if (activePersonListItem == null)
                    return;

                context.ActivePeopleAssignedToBodyMandatesList.Remove(activePersonListItem);

                context.SaveChanges();
            }
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = _contextFactory().Value)
            {
                return context.ActivePeopleAssignedToBodyMandatesList
                    .Where(item => item.ValidTo.HasValue)
                    .Where(item => item.ValidTo.Value <= message.Body.Date)
                    .Select(item =>
                            new UpdateCurrentPersonAssignedToBodyMandate(
                                new BodyId(item.BodyId),
                                new BodySeatId(item.BodySeatId),
                                new BodyMandateId(item.BodyMandateId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
