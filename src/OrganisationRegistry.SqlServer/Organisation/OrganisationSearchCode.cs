namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

/// <summary>
/// Herkomst van een doorzoekbare code. Bewaard als tekst zodat de tabel leesbaar blijft bij
/// diagnose; de query filtert op <see cref="OrganisationSearchCodeListItem.KeyTypeId" />.
/// </summary>
public static class OrganisationSearchCodeSource
{
    public const string Ovo = "Ovo";
    public const string Kbo = "Kbo";
    public const string Key = "Key";

    public const int MaxLength = 10;
}

/// <summary>
/// Eén doorzoekbare code van een organisatie (OR-3785): het OVO-nummer, het KBO-nummer, of de
/// waarde van een organisatiesleutel.
/// <br />
/// Alle sleuteltypes worden bewaard, niet enkel Orafin: zo blijft dit read model onafhankelijk van
/// de instelling <c>Api:Orafin_KeyTypeId</c> en volgt een wijziging van die instelling
/// onmiddellijk, zonder herbouw van de projectie.
/// <br />
/// <see cref="ValidFrom" /> en <see cref="ValidTo" /> zijn informatief: het zoeken toetst de
/// geldigheid van een sleutel niet, zodat ook vervallen sleutels gevonden worden en het
/// zoekresultaat datum-onafhankelijk is.
/// </summary>
public class OrganisationSearchCodeListItem
{
    public int Id { get; set; }

    public Guid OrganisationId { get; set; }

    /// <summary>Eén van de waarden uit <see cref="OrganisationSearchCodeSource" />.</summary>
    public string Source { get; set; } = null!;

    /// <summary>Gevuld voor <see cref="OrganisationSearchCodeSource.Key" />, null voor OVO en KBO.</summary>
    public Guid? KeyTypeId { get; set; }

    /// <summary>Identificeert de organisatiesleutel bij wijzigen en verwijderen.</summary>
    public Guid? OrganisationKeyId { get; set; }

    /// <summary>De waarde zoals ze in het register staat.</summary>
    public string Value { get; set; } = null!;

    /// <summary>De waarde waarop gezocht wordt, genormaliseerd.</summary>
    public string NormalizedValue { get; set; } = null!;

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class OrganisationSearchCodeListConfiguration : EntityMappingConfiguration<OrganisationSearchCodeListItem>
{
    public const int ValueLength = OrganisationKeyListConfiguration.KeyValueLength;

    /// <summary>
    /// Binaire collatie voor <see cref="OrganisationSearchCodeListItem.NormalizedValue" />. De waarde is al
    /// genormaliseerd (hoofdletters, enkel letters en cijfers, zie <see cref="OrganisationCodeNormalizer" />), dus
    /// een hoofdletterongevoelige collatie voegt niets toe, terwijl ze een deelmatch (<c>LIKE '%x%'</c>) een
    /// veelvoud duurder maakt: elke vergelijking volgt dan de Unicode-regels in plaats van een vergelijking per
    /// teken. Wie deze kolom ooit vergelijkt met een kolom in een andere collatie, moet die collatie expliciet
    /// maken.
    /// </summary>
    public const string NormalizedValueCollation = "Latin1_General_100_BIN2";

    public override void Map(EntityTypeBuilder<OrganisationSearchCodeListItem> b)
    {
        b.ToTable(nameof(OrganisationSearchListItemView.ProjectionTables.OrganisationSearchCodeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Id).UseIdentityColumn();

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.Source).HasMaxLength(OrganisationSearchCodeSource.MaxLength).IsRequired();

        b.Property(p => p.KeyTypeId);
        b.Property(p => p.OrganisationKeyId);

        b.Property(p => p.Value).HasMaxLength(ValueLength).IsRequired();
        b.Property(p => p.NormalizedValue).HasMaxLength(ValueLength).IsRequired()
            .UseCollation(NormalizedValueCollation);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        // Bedient het code-filter. Dat zoekt enkel in OVO- en KBO-codes (KeyTypeId null) en in het Orafin-sleuteltype;
        // met KeyTypeId vooraan leest SQL Server enkel die twee bereiken, in plaats van de sleutels van alle
        // sleuteltypes. Binnen die bereiken blijft de deelmatch (LIKE '%x%') een scan: een leidende wildcard laat geen
        // seek toe. OrganisationId zit erbij ingesloten zodat de tabel (een heap) niet geraadpleegd hoeft te worden.
        b.HasIndex(x => new { x.KeyTypeId, x.NormalizedValue })
            .IncludeProperties(x => x.OrganisationId);

        b.HasIndex(x => x.OrganisationId);
        b.HasIndex(x => x.OrganisationKeyId);
    }
}
