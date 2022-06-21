namespace OrganisationRegistry.SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

public class ActivePeopleAssignedToBodyMandateListItem
{
    public Guid DelegationAssignmentId { get; set; }

    public Guid PersonId { get; set; }

    public string PersonFullName { get; set; } = null!;

    public Guid BodySeatId { get; set; }

    public Guid BodyMandateId { get; set; }

    public Guid BodyId { get; set; }

    public DateTime? ValidTo { get; set; }
}

public class ActivePeopleAssignedToBodyMandatesListConfiguration : EntityMappingConfiguration<ActivePeopleAssignedToBodyMandateListItem>
{
    public override void Map(EntityTypeBuilder<ActivePeopleAssignedToBodyMandateListItem> b)
    {
        b.ToTable(nameof(ActivePeopleAssignedToBodyMandatesListView.ProjectionTables.ActivePeopleAssignedToBodyMandatesList), WellknownSchemas.BackofficeSchema)
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
    IEventHandler<AssignedPersonClearedFromBodyMandate>
{
    private readonly IEventStore _eventStore;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ActivePeopleAssignedToBodyMandatesListView(
        ILogger<ActivePeopleAssignedToBodyMandatesListView> logger,
        IEventStore eventStore,
        IDateTimeProvider dateTimeProvider,
        IContextFactory contextFactory
    ) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        ActivePeopleAssignedToBodyMandatesList,
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
    {
        var validTo = new ValidTo(message.Body.ValidTo);

        if (validTo.IsInPastOf(_dateTimeProvider.Today))
            return;

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var activePerson =
                context.ActivePeopleAssignedToBodyMandatesList
                    .SingleOrDefault(item => item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

            if (activePerson == null)
                return;

            activePerson.PersonId = message.Body.PersonId;
            activePerson.PersonFullName = message.Body.PersonFullName;
            activePerson.ValidTo = validTo;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonAssignedToBodyMandate> message)
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
            ValidTo = validTo,
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.ActivePeopleAssignedToBodyMandatesList.AddAsync(activePersonListItem);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonClearedFromBodyMandate> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var activePersonListItem =
            context.ActivePeopleAssignedToBodyMandatesList
                .SingleOrDefault(item =>
                    item.BodyId == message.Body.BodyId &&
                    item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

        if (activePersonListItem == null)
            return;

        context.ActivePeopleAssignedToBodyMandatesList.Remove(activePersonListItem);

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
