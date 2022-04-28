namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class BodyDetail
    {
        public Guid Id { get; set; }

        public string BodyNumber { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? ShortName { get; set; }
        public string? Description { get; set; }

        public DateTime? FormalValidFrom { get; set; }
        public DateTime? FormalValidTo { get; set; }

        public string? Organisation { get; set; }
        public Guid? OrganisationId { get; set; }

        public bool IsLifecycleValid { get; set; }

        public bool? IsBalancedParticipationObligatory { get; set; }
        public string? BalancedParticipationExtraRemark { get; set; }
        public string? BalancedParticipationExceptionMeasure { get; set; }
    }

    public class BodyDetailConfiguration : EntityMappingConfiguration<BodyDetail>
    {
        public override void Map(EntityTypeBuilder<BodyDetail> b)
        {
            b.ToTable(nameof(BodyDetailView.ProjectionTables.BodyDetail), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.BodyNumber)
                .HasMaxLength(BodyListConfiguration.BodyNumberLength)
                .IsRequired();

            b.Property(p => p.Name)
                .HasMaxLength(BodyListConfiguration.NameLength)
                .IsRequired();

            b.Property(p => p.ShortName);
            b.Property(p => p.Description);

            b.Property(p => p.FormalValidFrom);
            b.Property(p => p.FormalValidTo);

            b.Property(p => p.Organisation).HasMaxLength(OrganisationListConfiguration.NameLength);
            b.Property(p => p.OrganisationId);

            b.Property(p => p.IsLifecycleValid);

            b.Property(p => p.IsBalancedParticipationObligatory);
            b.Property(p => p.BalancedParticipationExtraRemark);
            b.Property(p => p.BalancedParticipationExceptionMeasure);

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.ShortName);
            b.HasIndex(x => x.FormalValidFrom);
            b.HasIndex(x => x.FormalValidTo);
            b.HasIndex(x => x.Organisation);
        }
    }

    public class BodyDetailView :
        Projection<BodyDetailView>,
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyNumberAssigned>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodyFormalValidityChanged>,
        IEventHandler<BodyLifecycleBecameValid>,
        IEventHandler<BodyLifecycleBecameInvalid>,
        IEventHandler<BodyBalancedParticipationChanged>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationNameUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>

    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            BodyDetail
        }

        private readonly IEventStore _eventStore;
        public BodyDetailView(
            ILogger<BodyDetailView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyRegistered> message)
        {
            var bodyDetailItem = new BodyDetail
            {
                Id = message.Body.BodyId,
                Name = message.Body.Name,
                BodyNumber = message.Body.BodyNumber,
                ShortName = message.Body.ShortName,
                Description = message.Body.Description,
                FormalValidFrom = message.Body.FormalValidFrom,
                FormalValidTo = message.Body.FormalValidTo,
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.BodyDetail.AddAsync(bodyDetailItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyNumberAssigned> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.BodyNumber = message.Body.BodyNumber;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.OrganisationId = message.Body.OrganisationId;
            bodyDetailItem.Organisation = message.Body.OrganisationName;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.OrganisationId = null;
            bodyDetailItem.Organisation = null;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.OrganisationId = message.Body.OrganisationId;
            bodyDetailItem.Organisation = message.Body.OrganisationName;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.Name = message.Body.Name;
            bodyDetailItem.ShortName = message.Body.ShortName;
            bodyDetailItem.Description = message.Body.Description;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyFormalValidityChanged> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.FormalValidFrom = message.Body.FormalValidFrom;
            bodyDetailItem.FormalValidTo = message.Body.FormalValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecycleBecameValid> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.IsLifecycleValid = true;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecycleBecameInvalid> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.IsLifecycleValid = false;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyBalancedParticipationChanged> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItem = context.BodyDetail.Single(item => item.Id == message.Body.BodyId);

            bodyDetailItem.IsBalancedParticipationObligatory = message.Body.BalancedParticipationObligatory;
            bodyDetailItem.BalancedParticipationExtraRemark = message.Body.BalancedParticipationExtraRemark;
            bodyDetailItem.BalancedParticipationExceptionMeasure = message.Body.BalancedParticipationExceptionMeasure;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItemDetails = context.BodyDetail.Where(item => item.OrganisationId == message.Body.OrganisationId);

            foreach (var bodyDetail in bodyDetailItemDetails)
            {
                bodyDetail.Organisation = message.Body.Name;
            }

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItemDetails = context.BodyDetail.Where(item => item.OrganisationId == message.Body.OrganisationId);

            foreach (var bodyDetail in bodyDetailItemDetails)
            {
                bodyDetail.Organisation = message.Body.Name;
            }

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var bodyDetailItemDetails = context.BodyDetail.Where(item => item.OrganisationId == message.Body.OrganisationId);

            foreach (var bodyDetail in bodyDetailItemDetails)
            {
                bodyDetail.Organisation = message.Body.Name;
            }

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
