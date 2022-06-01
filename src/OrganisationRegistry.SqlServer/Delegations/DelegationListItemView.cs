namespace OrganisationRegistry.SqlServer.Delegations;

using System;
using Body;
using FunctionType;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Organisation;
using OrganisationRegistry.Infrastructure;

public class DelegationListItem
{
    public Guid Id { get; set; }

    public Guid OrganisationId { get; set; } // Organisatie aan wie het mandaat is toegekend
    public string? OrganisationName { get; set; }

    public Guid? FunctionTypeId { get; set; } // Functietype die nodig is voor het mandaat uit te voeren
    public string? FunctionTypeName { get; set; }

    public Guid BodyId { get; set; } // Het orgaan zelf
    public string? BodyName { get; set; }

    public Guid? BodyOrganisationId { get; set; } // Organisatie aan wie het orgaan behoort (DayPassed)
    public string? BodyOrganisationName { get; set; }

    public Guid BodySeatId { get; set; } // Het postje die ingevuld wordt
    public string? BodySeatName { get; set; }
    public string? BodySeatNumber { get; set; }
    public Guid? BodySeatTypeId { get; set; }
    public string? BodySeatTypeName { get; set; }


    public bool IsDelegated { get; set; }
    public int NumberOfDelegationAssignments { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class DelegationListConfiguration : EntityMappingConfiguration<DelegationListItem>
{
    public const string TableName = "DelegationList";

    public override void Map(EntityTypeBuilder<DelegationListItem> b)
    {
        b.ToTable(TableName, WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.OrganisationId);
        b.Property(p => p.OrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength);

        b.Property(p => p.FunctionTypeId);
        b.Property(p => p.FunctionTypeName).HasMaxLength(FunctionTypeListConfiguration.NameLength);

        b.Property(p => p.BodyId);
        b.Property(p => p.BodyName).HasMaxLength(BodyListConfiguration.NameLength);
        b.Property(p => p.BodyOrganisationId);
        b.Property(p => p.BodyOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength);

        b.Property(p => p.BodySeatName).HasMaxLength(BodySeatListConfiguration.NameLength);
        b.Property(p => p.BodySeatNumber).HasMaxLength(BodySeatListConfiguration.SeatNumberLength);

        b.Property(p => p.IsDelegated);
        b.Property(p => p.NumberOfDelegationAssignments);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.OrganisationName).IsClustered();
        b.HasIndex(x => x.BodyName);
        b.HasIndex(x => x.BodySeatName);
        b.HasIndex(x => x.BodySeatNumber);
        b.HasIndex(x => x.BodySeatTypeId);
        b.HasIndex(x => x.BodySeatTypeName);
        b.HasIndex(x => x.IsDelegated);
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}
