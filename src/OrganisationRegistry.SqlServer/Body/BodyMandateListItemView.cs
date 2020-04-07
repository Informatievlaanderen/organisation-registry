namespace OrganisationRegistry.SqlServer.Body
{
    using Function.Events;
    using FunctionType;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Organisation;
    using Person;
    using SeatType;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Person.Events;
    using OrganisationRegistry.SeatType.Events;

    public class BodyMandateListItem
    {
        public Guid BodyMandateId { get; set; }
        public BodyMandateType BodyMandateType { get; set; }
        public Guid BodyId { get; set; }

        public Guid BodySeatId { get; set; }
        public string BodySeatNumber { get; set; }
        public string BodySeatName { get; set; }

        public Guid? BodySeatTypeId { get; set; }
        public string? BodySeatTypeName { get; set; }
        public int? BodySeatTypeOrder { get; set; }

        public Guid DelegatorId { get; set; }
        public string DelegatorName { get; set; }

        public Guid? DelegatedId { get; set; }
        public string? DelegatedName { get; set; }

        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public string ContactsJson { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodyMandateListConfiguration : EntityMappingConfiguration<BodyMandateListItem>
    {
        public int RepresentationLength = new[] {
            OrganisationListConfiguration.NameLength,
            FunctionTypeListConfiguration.NameLength,
            PersonListConfiguration.FullNameLength
        }.Max();

        public override void Map(EntityTypeBuilder<BodyMandateListItem> b)
        {
            b.ToTable(nameof(BodyMandateListView.ProjectionTables.BodyMandateList), "OrganisationRegistry")
                .HasKey(p => p.BodyMandateId)
                .IsClustered(false);

            b.Property(p => p.BodyMandateType).IsRequired();

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.BodySeatId).IsRequired();

            b.Property(p => p.BodySeatNumber)
                .HasMaxLength(BodySeatListConfiguration.SeatNumberLength);

            b.Property(p => p.BodySeatName)
                .HasMaxLength(BodySeatListConfiguration.NameLength)
                .IsRequired();

            b.Property(p => p.BodySeatTypeId);

            b.Property(p => p.BodySeatTypeName)
                .HasMaxLength(SeatTypeListConfiguration.NameLength);

            b.Property(p => p.BodySeatTypeOrder);

            b.Property(p => p.DelegatorId).IsRequired();
            b.Property(p => p.DelegatorName)
                .HasMaxLength(RepresentationLength)
                .IsRequired();

            b.Property(p => p.DelegatedId);
            b.Property(p => p.DelegatedName)
                .HasMaxLength(RepresentationLength);

            b.Property(p => p.AssignedToId);
            b.Property(p => p.AssignedToName)
                .HasMaxLength(PersonListConfiguration.FullNameLength);

            b.Property(p => p.ContactsJson);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.DelegatorName).IsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
            b.HasIndex(x => x.BodySeatName);
        }
    }

    public class BodyMandateListView :
        Projection<BodyMandateListView>,
        IEventHandler<AssignedPersonToBodySeat>,
        IEventHandler<AssignedFunctionTypeToBodySeat>,
        IEventHandler<AssignedOrganisationToBodySeat>,
        IEventHandler<ReassignedPersonToBodySeat>,
        IEventHandler<ReassignedFunctionTypeToBodySeat>,
        IEventHandler<ReassignedOrganisationToBodySeat>,
        IEventHandler<PersonUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<SeatTypeUpdated>,
        IEventHandler<BodySeatNumberAssigned>,
        IEventHandler<AssignedPersonAssignedToBodyMandate>,
        IEventHandler<AssignedPersonClearedFromBodyMandate>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyMandateList
        }

        private readonly IEventStore _eventStore;
        private Func<DbConnection, DbTransaction, OrganisationRegistryContext> _contextFactory;

        public BodyMandateListView(
            ILogger<BodyMandateListView> logger,
            IEventStore eventStore,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory = null) : base(logger)
        {
            _eventStore = eventStore;
            _contextFactory = contextFactory ?? ((connection, transaction) =>
                new OrganisationRegistryTransactionalContext(connection, transaction));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            var bodyMandateListItem = new BodyMandateListItem
            {
                BodyMandateId = message.Body.BodyMandateId,
                BodyMandateType = BodyMandateType.Person,
                BodyId = message.Body.BodyId,

                BodySeatId = message.Body.BodySeatId,
                BodySeatName = message.Body.BodySeatName,
                BodySeatNumber = message.Body.BodySeatNumber,

                //BodySeatTypeId = message.Body.BodySeatTypeId,
                //BodySeatTypeName = message.Body.BodySeatTypeName,
                //BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

                DelegatorId = message.Body.PersonId,
                DelegatorName = FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName),
                DelegatedId = null,
                DelegatedName = null,

                ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>()),

                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.BodyMandateList.Add(bodyMandateListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedFunctionTypeToBodySeat> message)
        {
            var bodyMandateListItem = new BodyMandateListItem
            {
                BodyMandateId = message.Body.BodyMandateId,
                BodyMandateType = BodyMandateType.FunctionType,
                BodyId = message.Body.BodyId,

                BodySeatId = message.Body.BodySeatId,
                BodySeatName = message.Body.BodySeatName,
                BodySeatNumber = message.Body.BodySeatNumber,

                //BodySeatTypeId = message.Body.BodySeatTypeId,
                //BodySeatTypeName = message.Body.BodySeatTypeName,
                BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

                DelegatorId = message.Body.OrganisationId,
                DelegatorName = message.Body.OrganisationName,
                DelegatedId = message.Body.FunctionTypeId,
                DelegatedName = message.Body.FunctionTypeName,

                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.BodyMandateList.Add(bodyMandateListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            var bodyMandateListItem = new BodyMandateListItem
            {
                BodyMandateId = message.Body.BodyMandateId,
                BodyMandateType = BodyMandateType.Organisation,
                BodyId = message.Body.BodyId,

                BodySeatId = message.Body.BodySeatId,
                BodySeatName = message.Body.BodySeatName,
                BodySeatNumber = message.Body.BodySeatNumber,

                //BodySeatTypeId = message.Body.BodySeatTypeId,
                //BodySeatTypeName = message.Body.BodySeatTypeName,
                BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

                DelegatorId = message.Body.OrganisationId,
                DelegatorName = message.Body.OrganisationName,
                DelegatedId = null,
                DelegatedName = null,

                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.BodyMandateList.Add(bodyMandateListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandateListItem = context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
                bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
                bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

                //bodyMandateListItem.BodySeatTypeId = message.Body.BodySeatTypeId;
                //bodyMandateListItem.BodySeatTypeName = message.Body.BodySeatTypeName;
                bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

                bodyMandateListItem.DelegatorId = message.Body.PersonId;
                bodyMandateListItem.DelegatorName = FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName);
                bodyMandateListItem.DelegatedId = null;
                bodyMandateListItem.DelegatedName = null;

                bodyMandateListItem.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>());
                bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
                bodyMandateListItem.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandateListItem = context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
                bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
                bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

                //bodyMandateListItem.BodySeatTypeId = message.Body.BodySeatTypeId;
                //bodyMandateListItem.BodySeatTypeName = message.Body.BodySeatTypeName;
                bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

                bodyMandateListItem.DelegatorId = message.Body.OrganisationId;
                bodyMandateListItem.DelegatorName = message.Body.OrganisationName;
                bodyMandateListItem.DelegatedId = message.Body.FunctionTypeId;
                bodyMandateListItem.DelegatedName = message.Body.FunctionTypeName;

                bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
                bodyMandateListItem.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandateListItem = context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
                bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
                bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

                //bodyMandateListItem.BodySeatTypeId = message.Body.BodySeatTypeId;
                //bodyMandateListItem.BodySeatTypeName = message.Body.BodySeatTypeName;
                bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

                bodyMandateListItem.DelegatorId = message.Body.OrganisationId;
                bodyMandateListItem.DelegatorName = message.Body.OrganisationName;
                bodyMandateListItem.DelegatedId = null;
                bodyMandateListItem.DelegatedName = null;

                bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
                bodyMandateListItem.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonAssignedToBodyMandate> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandateListItem = context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                bodyMandateListItem.AssignedToId = message.Body.PersonId;
                bodyMandateListItem.AssignedToName = message.Body.PersonFullName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonClearedFromBodyMandate> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandateListItem = context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

                bodyMandateListItem.AssignedToId = null;
                bodyMandateListItem.AssignedToName = string.Empty;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item => item.BodyMandateType == BodyMandateType.Person && item.DelegatorId == message.Body.PersonId);

                foreach (var bodyMandateListItem in bodyMandates)
                    bodyMandateListItem.DelegatorName = FormatPersonName(message.Body.FirstName, message.Body.Name);

                var bodyDelegationAssignments = context
                    .BodyMandateList
                    .Where(item => item.AssignedToId == message.Body.PersonId);

                foreach (var bodyDelegationAssignment in bodyDelegationAssignments)
                    bodyDelegationAssignment.AssignedToName = FormatPersonName(message.Body.FirstName, message.Body.Name);

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item => item.BodyMandateType == BodyMandateType.FunctionType && item.DelegatedId == message.Body.FunctionId);

                foreach (var bodyMandateListItem in bodyMandates)
                    bodyMandateListItem.DelegatedName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateDelegatorName(dbConnection, dbTransaction, _contextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateDelegatorName(dbConnection, dbTransaction, _contextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        private static void UpdateDelegatorName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory,
            Guid organisationId,
            string delegatorName)
        {
            using (var context = contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item =>
                        (item.BodyMandateType == BodyMandateType.Organisation ||
                         item.BodyMandateType == BodyMandateType.FunctionType) &&
                        item.DelegatorId == organisationId);

                foreach (var bodyMandateListItem in bodyMandates)
                    bodyMandateListItem.DelegatorName = delegatorName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item => item.BodySeatId == message.Body.BodySeatId);

                foreach (var bodyMandateListItem in bodyMandates)
                {
                    bodyMandateListItem.BodySeatName = message.Body.Name;
                    bodyMandateListItem.BodySeatTypeId = message.Body.SeatTypeId;
                    bodyMandateListItem.BodySeatTypeName = message.Body.SeatTypeName;
                    bodyMandateListItem.BodySeatTypeOrder = message.Body.SeatTypeOrder ?? int.MaxValue;
                }

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item => item.BodySeatTypeId == message.Body.SeatTypeId);

                foreach (var bodyMandateListItem in bodyMandates)
                {
                    bodyMandateListItem.BodySeatTypeId = message.Body.SeatTypeId;
                    bodyMandateListItem.BodySeatTypeName = message.Body.Name;
                    bodyMandateListItem.BodySeatTypeOrder = message.Body.Order ?? int.MaxValue;
                }

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatNumberAssigned> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var bodyMandates = context
                    .BodyMandateList
                    .Where(item => item.BodySeatId == message.Body.BodySeatId);

                foreach (var bodyMandateListItem in bodyMandates)
                    bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static string FormatPersonName(string firstName, string name)
        {
            return $"{firstName} {name}";
        }
    }
}
