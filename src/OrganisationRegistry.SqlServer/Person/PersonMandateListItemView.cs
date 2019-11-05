namespace OrganisationRegistry.SqlServer.Person
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Body;
    using FunctionType;
    using Organisation;

    public class PersonMandateListItem
    {
        public Guid PersonMandateId { get; set; } // arbitrary guid as pk

        public Guid BodyMandateId { get; set; }
        public Guid? DelegationAssignmentId { get; set; }

        public Guid BodyId { get; set; } // Het orgaan zelf
        public string BodyName { get; set; }

        public Guid? BodyOrganisationId { get; set; } // Organisatie aan wie het orgaan behoort (DayPassed)
        public string BodyOrganisationName { get; set; }

        public Guid BodySeatId { get; set; }
        public string BodySeatName { get; set; }
        public string BodySeatNumber { get; set; }

        public bool PaidSeat { get; set; }

        public Guid? OrganisationId { get; set; } // Organisatie aan wie het mandaat is toegekend
        public string OrganisationName { get; set; }

        public Guid? FunctionTypeId { get; set; } // Functietype die nodig is voor het mandaat uit te voeren
        public string FunctionTypeName { get; set; }

        public Guid PersonId { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class PersonMandateListConfiguration : EntityMappingConfiguration<PersonMandateListItem>
    {
        public const string TableName = "PersonMandateList";

        public override void Map(EntityTypeBuilder<PersonMandateListItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.PersonMandateId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.DelegationAssignmentId).IsRequired(false);

            b.HasIndex(p => new {p.BodyMandateId, p.DelegationAssignmentId}).IsUnique();

            b.Property(p => p.BodyId).IsRequired();
            b.Property(p => p.BodyName).HasMaxLength(BodyListConfiguration.NameLength).IsRequired();

            b.Property(p => p.BodyOrganisationId);
            b.Property(p => p.BodyOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength);

            b.Property(p => p.BodySeatId).IsRequired();
            b.Property(p => p.BodySeatName).HasMaxLength(BodySeatListConfiguration.NameLength).IsRequired();
            b.Property(p => p.BodySeatNumber).HasMaxLength(BodySeatListConfiguration.SeatNumberLength);

            b.Property(p => p.OrganisationId);
            b.Property(p => p.OrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength);

            b.Property(p => p.FunctionTypeId);
            b.Property(p => p.FunctionTypeName).HasMaxLength(FunctionTypeListConfiguration.NameLength);

            b.Property(p => p.PersonId).IsRequired();

            b.Property(p => p.PaidSeat);

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.BodyName).ForSqlServerIsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }
}
