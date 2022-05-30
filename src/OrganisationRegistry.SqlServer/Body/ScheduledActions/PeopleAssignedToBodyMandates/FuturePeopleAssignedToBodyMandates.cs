﻿namespace OrganisationRegistry.SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates;

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

public class FuturePeopleAssignedToBodyMandatesListItem
{
    public Guid DelegationAssignmentId { get; set; }

    public Guid PersonId { get; set; }

    public string PersonFullName { get; set; } = null!;

    public Guid BodySeatId { get; set; }

    public Guid BodyMandateId { get; set; }

    public Guid BodyId { get; set; }

    public DateTime? ValidFrom { get; set; }
}

public class FuturePeopleAssignedToBodyMandatesListConfiguration : EntityMappingConfiguration<FuturePeopleAssignedToBodyMandatesListItem>
{
    public override void Map(EntityTypeBuilder<FuturePeopleAssignedToBodyMandatesListItem> b)
    {
        b.ToTable(nameof(FuturePeopleAssignedToBodyMandatesListView.ProjectionTables.FuturePeopleAssignedToBodyMandatesList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.DelegationAssignmentId)
            .IsClustered(false);

        b.Property(p => p.PersonId).IsRequired();

        b.Property(p => p.PersonFullName).IsRequired();

        b.Property(p => p.BodySeatId).IsRequired();

        b.Property(p => p.BodyMandateId).IsRequired();

        b.Property(p => p.BodyId).IsRequired();

        b.Property(p => p.ValidFrom);

        b.HasIndex(x => x.ValidFrom);
    }
}

public class FuturePeopleAssignedToBodyMandatesListView :
    Projection<FuturePeopleAssignedToBodyMandatesListView>,
    IEventHandler<PersonAssignedToDelegation>,
    IEventHandler<PersonAssignedToDelegationUpdated>,
    IEventHandler<PersonAssignedToDelegationRemoved>,
    IEventHandler<AssignedPersonAssignedToBodyMandate>
{
    private readonly IEventStore _eventStore;
    private readonly IDateTimeProvider _dateTimeProvider;

    public FuturePeopleAssignedToBodyMandatesListView(
        ILogger<FuturePeopleAssignedToBodyMandatesListView> logger,
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
        FuturePeopleAssignedToBodyMandatesList
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
    {
        var validFrom = new ValidFrom(message.Body.ValidFrom);
        if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        InsertFutureActivePerson(context, message);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var validFrom = new ValidFrom(message.Body.ValidFrom);
        if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
        {
            DeleteFutureActivePerson(context, message.Body.DelegationAssignmentId);
        }
        else
        {
            UpsertFutureActivePerson(context, message);
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonAssignedToBodyMandate> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        DeleteFutureActivePerson(context, message.Body.DelegationAssignmentId);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        DeleteFutureActivePerson(context, message.Body.DelegationAssignmentId);
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    private static void InsertFutureActivePerson(
        OrganisationRegistryContext context,
        IEnvelope<PersonAssignedToDelegation> message)
    {
        var futureActivePersonListItem = new FuturePeopleAssignedToBodyMandatesListItem
        {
            DelegationAssignmentId = message.Body.DelegationAssignmentId,
            BodySeatId = message.Body.BodySeatId,
            BodyMandateId = message.Body.BodyMandateId,
            BodyId = message.Body.BodyId,
            PersonId = message.Body.PersonId,
            PersonFullName = message.Body.PersonFullName,
            ValidFrom = message.Body.ValidFrom
        };

        context.FuturePeopleAssignedToBodyMandatesList.Add(futureActivePersonListItem);
        context.SaveChanges();
    }

    private static void UpsertFutureActivePerson(
        OrganisationRegistryContext context,
        IEnvelope<PersonAssignedToDelegationUpdated> message)
    {
        var futureActivePerson =
            context.FuturePeopleAssignedToBodyMandatesList.SingleOrDefault(
                item => item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

        if (futureActivePerson == null)
        {
            var futureActivePersonListItem =
                new FuturePeopleAssignedToBodyMandatesListItem
                {
                    DelegationAssignmentId = message.Body.DelegationAssignmentId,
                    BodySeatId = message.Body.BodySeatId,
                    BodyMandateId = message.Body.BodyMandateId,
                    BodyId = message.Body.BodyId,
                    PersonId = message.Body.PersonId,
                    PersonFullName = message.Body.PersonFullName,
                    ValidFrom = message.Body.ValidFrom
                };

            context.FuturePeopleAssignedToBodyMandatesList.Add(futureActivePersonListItem);
        }
        else
        {
            futureActivePerson.PersonId = message.Body.PersonId;
            futureActivePerson.PersonFullName = message.Body.PersonFullName;
            futureActivePerson.ValidFrom = message.Body.ValidFrom;
        }

        context.SaveChanges();
    }

    private static void DeleteFutureActivePerson(
        OrganisationRegistryContext context,
        Guid delegationAssignmentId)
    {
        var futureActivePerson =
            context.FuturePeopleAssignedToBodyMandatesList.SingleOrDefault(
                item => item.DelegationAssignmentId == delegationAssignmentId);

        if (futureActivePerson == null)
            return;

        context.FuturePeopleAssignedToBodyMandatesList.Remove(futureActivePerson);
        context.SaveChanges();
    }
}