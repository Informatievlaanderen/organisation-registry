namespace OrganisationRegistry.SqlServer.Person
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Person.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Person;

    public class PersonListItem
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public Sex? Sex { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }

    public class PersonListConfiguration : EntityMappingConfiguration<PersonListItem>
    {
        public const int FirstNameLength = 200;
        public const int NameLength = 200;
        public const int FullNameLength = FirstNameLength + NameLength + 1; // 1 for space

        public override void Map(EntityTypeBuilder<PersonListItem> b)
        {
            b.ToTable(nameof(PersonListView.ProjectionTables.PersonList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.FirstName)
                .HasMaxLength(FirstNameLength)
                .IsRequired();

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.FullName)
                .HasMaxLength(FullNameLength)
                .IsRequired();

            b.Property(p => p.Sex);
            b.Property(p => p.DateOfBirth);

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.FirstName);
            b.HasIndex(x => x.FullName);
        }
    }

    public class PersonListView :
        Projection<PersonListView>,
        IEventHandler<PersonCreated>,
        IEventHandler<PersonUpdated>
    {
        private readonly IEventStore _eventStore;
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            PersonList
        }

        public PersonListView(
            ILogger<PersonListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var person = new PersonListItem
                {
                    Id = message.Body.PersonId,
                    FirstName = message.Body.FirstName,
                    Name = message.Body.Name,
                    FullName = $"{message.Body.FirstName} {message.Body.Name}",
                    Sex = message.Body.Sex,
                    DateOfBirth = message.Body.DateOfBirth
                };

                await context.PersonList.AddAsync(person);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var person = context.PersonList.SingleOrDefault(x => x.Id == message.Body.PersonId);
                if (person == null)
                    return; // TODO: Error?

                person.FirstName = message.Body.FirstName;
                person.Name = message.Body.Name;
                person.FullName = $"{message.Body.FirstName} {message.Body.Name}";
                person.Sex = message.Body.Sex;
                person.DateOfBirth = message.Body.DateOfBirth;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
