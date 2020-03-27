namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.LifecyclePhaseType;

    public class BodyListItem
    {
        public Guid Id { get; set; }

        public string BodyNumber { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public string? Organisation { get; set; }
        public Guid? OrganisationId { get; set; }

        public List<BodyLifecyclePhaseValidity> BodyLifecyclePhaseValidities { get; set; } = new List<BodyLifecyclePhaseValidity>();
    }

    public class BodyLifecyclePhaseValidity
    {
        public Guid BodyId { get; set; }
        public Guid BodyLifecyclePhaseId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool RepresentsActivePhase { get; set; }
    }

    public class BodyListConfiguration : EntityMappingConfiguration<BodyListItem>
    {
        public const int NameLength = 500;
        public const int BodyNumberLength = 10;

        public override void Map(EntityTypeBuilder<BodyListItem> b)
        {
            b.ToTable(nameof(BodyListView.ProjectionTables.BodyList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.BodyNumber)
                .HasMaxLength(BodyNumberLength);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.ShortName);

            b.Property(p => p.Organisation).HasMaxLength(OrganisationListConfiguration.NameLength);
            b.Property(p => p.OrganisationId);

            b.HasMany(p => p.BodyLifecyclePhaseValidities).WithOne().OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.ShortName);
            b.HasIndex(x => x.Organisation);
            b.HasIndex(x => x.BodyNumber);
        }
    }

    public class BodyLifecyclePhaseValidityListConfiguration : EntityMappingConfiguration<BodyLifecyclePhaseValidity>
    {
        public override void Map(EntityTypeBuilder<BodyLifecyclePhaseValidity> b)
        {
            b.ToTable(nameof(BodyListView.ProjectionTables.BodyLifecyclePhaseValidity), "OrganisationRegistry")
                .HasKey(p => p.BodyLifecyclePhaseId)
                .IsClustered(false);

            b.Property(p => p.BodyLifecyclePhaseId).ValueGeneratedNever();

            b.Property(p => p.BodyId);
            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);
            b.Property(p => p.RepresentsActivePhase);

            b.HasIndex(p => p.BodyId).IsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
            b.HasIndex(x => x.RepresentsActivePhase);
        }
    }

    public class BodyListView :
        Projection<BodyListView>,
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyNumberAssigned>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodyLifecyclePhaseAdded>,
        IEventHandler<BodyLifecyclePhaseUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyList,
            BodyLifecyclePhaseValidity
        }

        private readonly IEventStore _eventStore;

        public BodyListView(
            ILogger<BodyListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyRegistered> message)
        {
            var bodyListItem = new BodyListItem
            {
                Id = message.Body.BodyId,
                Name = message.Body.Name,
                ShortName = message.Body.ShortName,
                BodyNumber = message.Body.BodyNumber
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.BodyList.Add(bodyListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyNumberAssigned> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyListItem.BodyNumber = message.Body.BodyNumber;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyListItem.OrganisationId = message.Body.OrganisationId;
                bodyListItem.Organisation = message.Body.OrganisationName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyListItem.OrganisationId = null;
                bodyListItem.Organisation = null;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyDetailItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyDetailItem.OrganisationId = message.Body.OrganisationId;
                bodyDetailItem.Organisation = message.Body.OrganisationName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyListItem.Name = message.Body.Name;
                bodyListItem.ShortName = message.Body.ShortName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            var bodyLifecyclePhaseValidity = new BodyLifecyclePhaseValidity
            {
                BodyId = message.Body.BodyId,
                BodyLifecyclePhaseId = message.Body.BodyLifecyclePhaseId,
                ValidTo = message.Body.ValidTo,
                ValidFrom = message.Body.ValidFrom,
                RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context.BodyList.Single(item => item.Id == message.Body.BodyId);

                bodyListItem.BodyLifecyclePhaseValidities.Add(bodyLifecyclePhaseValidity);

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyListItem = context
                    .BodyList
                    .Include(x => x.BodyLifecyclePhaseValidities)
                    .Single(item => item.Id == message.Body.BodyId);

                var bodyLifecyclePhaseValidity = bodyListItem.BodyLifecyclePhaseValidities.Single(item => item.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId);

                bodyLifecyclePhaseValidity.BodyId = message.Body.BodyId;
                bodyLifecyclePhaseValidity.ValidTo = message.Body.ValidTo;
                bodyLifecyclePhaseValidity.ValidFrom = message.Body.ValidFrom;
                bodyLifecyclePhaseValidity.RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
