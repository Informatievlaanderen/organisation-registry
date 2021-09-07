namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    using System.Linq;
    using System.Threading.Tasks;
    using FunctionType;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Person;
    using OrganisationRegistry.Function.Events;
    using OrganisationRegistry.Person.Events;

    public class OrganisationFunctionListItem
    {
        public Guid OrganisationFunctionId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid FunctionId { get; set; }
        public string FunctionName { get; set; }

        public Guid PersonId { get; set; }
        public string PersonName { get; set; }

        public string? ContactsJson { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationFunctionListConfiguration : EntityMappingConfiguration<OrganisationFunctionListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationFunctionListItem> b)
        {
            b.ToTable(nameof(OrganisationFunctionListView.ProjectionTables.OrganisationFunctionList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.OrganisationFunctionId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.FunctionId).IsRequired();
            b.Property(p => p.FunctionName).HasMaxLength(FunctionTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.PersonId).IsRequired();
            b.Property(p => p.PersonName).HasMaxLength(PersonListConfiguration.FullNameLength).IsRequired();

            b.Property(p => p.ContactsJson);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.PersonName).IsClustered();
            b.HasIndex(x => x.FunctionName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationFunctionListView :
        Projection<OrganisationFunctionListView>,
        IEventHandler<OrganisationFunctionAdded>,
        IEventHandler<OrganisationFunctionUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<PersonUpdated>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationFunctionList
        }

        private readonly IEventStore _eventStore;
        public OrganisationFunctionListView(
            ILogger<OrganisationFunctionListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationFunctions = context.OrganisationFunctionList.Where(x => x.FunctionId == message.Body.FunctionId);
            if (!organisationFunctions.Any())
                return;

            foreach (var organisationFunction in organisationFunctions)
                organisationFunction.FunctionName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationFunctions = context.OrganisationFunctionList.Where(x => x.PersonId == message.Body.PersonId);
            if (!organisationFunctions.Any())
                return;

            foreach (var organisationFunction in organisationFunctions)
                organisationFunction.PersonName = $"{message.Body.FirstName} {message.Body.Name}";

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionAdded> message)
        {
            var organisationFunctionListItem = new OrganisationFunctionListItem
            {
                OrganisationFunctionId = message.Body.OrganisationFunctionId,
                OrganisationId = message.Body.OrganisationId,
                FunctionId = message.Body.FunctionId,
                PersonId = message.Body.PersonId,
                FunctionName = message.Body.FunctionName,
                PersonName = message.Body.PersonFullName,
                ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>()),
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationFunctionList.AddAsync(organisationFunctionListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var key = context.OrganisationFunctionList.SingleOrDefault(item => item.OrganisationFunctionId == message.Body.OrganisationFunctionId);

            key.OrganisationFunctionId = message.Body.OrganisationFunctionId;
            key.OrganisationId = message.Body.OrganisationId;
            key.FunctionId = message.Body.FunctionId;
            key.PersonId = message.Body.PersonId;
            key.FunctionName = message.Body.FunctionName;
            key.PersonName = message.Body.PersonFullName;
            key.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts ?? new Dictionary<Guid, string>());
            key.ValidFrom = message.Body.ValidFrom;
            key.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var functions = context.OrganisationFunctionList.Where(item =>
                message.Body.FieldsToTerminate.Functions.Keys.Contains(item.OrganisationFunctionId));

            foreach (var function in functions)
                function.ValidTo = message.Body.FieldsToTerminate.Functions[function.OrganisationFunctionId];

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var functions = context.OrganisationFunctionList.Where(item =>
                message.Body.FieldsToTerminate.Functions.Keys.Contains(item.OrganisationFunctionId));

            foreach (var function in functions)
                function.ValidTo = message.Body.FieldsToTerminate.Functions[function.OrganisationFunctionId];

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
