namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationBankAccountListItem
    {
        public Guid OrganisationBankAccountId { get; set; }
        public Guid OrganisationId { get; set; }
        public string BankAccountNumber { get; set; }
        public bool IsIban { get; set; }
        public string? Bic { get; set; }
        public bool IsBic { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public string? Source { get; set; }

        public bool IsEditable => Source != Sources.Kbo;

        public OrganisationBankAccountListItem() { }

        public OrganisationBankAccountListItem(
            Guid organisationBankAccountId,
            Guid organisationId,
            string bankAccountNumber,
            bool isIban,
            string bic,
            bool isBic,
            DateTime? validFrom,
            DateTime? validTo,
            string source = null)
        {
            OrganisationBankAccountId = organisationBankAccountId;
            OrganisationId = organisationId;
            BankAccountNumber = bankAccountNumber;
            IsIban = isIban;
            Bic = bic;
            IsBic = isBic;
            ValidFrom = validFrom;
            ValidTo = validTo;
            Source = source;
        }
    }

    public class OrganisationBankAccountListConfiguration : EntityMappingConfiguration<OrganisationBankAccountListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationBankAccountListItem> b)
        {
            b.ToTable(nameof(OrganisationBankAccountListView.ProjectionTables.OrganisationBankAccountList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationBankAccountId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.BankAccountNumber).IsRequired();
            b.Property(p => p.Bic);

            b.Property(p => p.IsIban);
            b.Property(p => p.IsBic);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.Source);

            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationBankAccountListView :
        Projection<OrganisationBankAccountListView>,
        IEventHandler<OrganisationBankAccountAdded>,
        IEventHandler<KboOrganisationBankAccountAdded>,
        IEventHandler<KboOrganisationBankAccountRemoved>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminationSyncedWithKbo>,
        IEventHandler<OrganisationBankAccountUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationBankAccountList
        }

        private readonly IEventStore _eventStore;
        public OrganisationBankAccountListView(
            ILogger<OrganisationBankAccountListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountAdded> message)
        {
            var organisationBankAccountListItem = new OrganisationBankAccountListItem(
                message.Body.OrganisationBankAccountId,
                message.Body.OrganisationId,
                message.Body.BankAccountNumber,
                message.Body.IsIban,
                message.Body.Bic,
                message.Body.IsBic,
                message.Body.ValidFrom,
                message.Body.ValidTo);

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationBankAccountList.AddAsync(organisationBankAccountListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountAdded> message)
        {
            var organisationBankAccountListItem = new OrganisationBankAccountListItem(
                message.Body.OrganisationBankAccountId,
                message.Body.OrganisationId,
                message.Body.BankAccountNumber,
                message.Body.IsIban,
                message.Body.Bic,
                message.Body.IsBic,
                message.Body.ValidFrom,
                message.Body.ValidTo,
                Sources.Kbo);

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationBankAccountList.AddAsync(organisationBankAccountListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboOrganisationBankAccountRemoved> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationBankAccountListItem = await context.OrganisationBankAccountList.SingleAsync(b => b.OrganisationBankAccountId == message.Body.OrganisationBankAccountId);

                context.OrganisationBankAccountList.Remove(organisationBankAccountListItem);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (!message.Body.OrganisationBankAccountIdsToCancel.Any())
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var bankAccountId in message.Body.OrganisationBankAccountIdsToCancel)
                {
                    var organisationBankAccountListItem = await context.OrganisationBankAccountList.SingleAsync(b => b.OrganisationBankAccountId == bankAccountId);

                    context.OrganisationBankAccountList.Remove(organisationBankAccountListItem);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (!message.Body.OrganisationBankAccountIdsToTerminate.Any())
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var bankAccountId in message.Body.OrganisationBankAccountIdsToTerminate)
                {
                    var organisationBankAccountListItem = await context.OrganisationBankAccountList.SingleAsync(b => b.OrganisationBankAccountId == bankAccountId);

                    organisationBankAccountListItem.ValidTo = message.Body.DateOfTermination;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBankAccountUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationBankAccountListItem = context.OrganisationBankAccountList.Single(b => b.OrganisationBankAccountId == message.Body.OrganisationBankAccountId);

                organisationBankAccountListItem.IsIban = message.Body.IsIban;
                organisationBankAccountListItem.BankAccountNumber = message.Body.BankAccountNumber;
                organisationBankAccountListItem.IsBic = message.Body.IsBic;
                organisationBankAccountListItem.Bic = message.Body.Bic;
                organisationBankAccountListItem.ValidFrom = message.Body.ValidFrom;
                organisationBankAccountListItem.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationBankAccountListItems =
                    context.OrganisationBankAccountList.Where(item => message.Body.FieldsToTerminate.BankAccounts.Keys.Contains(item.OrganisationBankAccountId));

                foreach (var bankAccount in organisationBankAccountListItems)
                    bankAccount.ValidTo = message.Body.FieldsToTerminate.BankAccounts[bankAccount.OrganisationBankAccountId];

                var kboOrganisationBankAccountListItems =
                    context.OrganisationBankAccountList.Where(item => message.Body.KboFieldsToTerminate.BankAccounts.Keys.Contains(item.OrganisationBankAccountId));

                foreach (var bankAccount in kboOrganisationBankAccountListItems)
                    bankAccount.ValidTo = message.Body.KboFieldsToTerminate.BankAccounts[bankAccount.OrganisationBankAccountId];

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
