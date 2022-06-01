namespace OrganisationRegistry.KboMutations;

using System;
using CsvHelper.Configuration.Attributes;

public class MutationsLine
{
    // schema available at https://vlaamseoverheid.atlassian.net/wiki/spaces/MG/pages/516129060/Interface+PubliceerOndernemingVKBO-02.00
    [Index(0)] public DateTime DatumModificatie { get; init; }

    [Index(13)] public string StatusCode { get; init; } = null!;

    [Index(1)] public string Ondernemingsnummer { get; init; } = null!;

    [Index(25)] public string MaatschappelijkeNaam { get; init; } = null!;

    [Index(83)] public DateTime? StopzettingsDatum { get; init; }

    [Index(84)] public string StopzettingsCode { get; init; } = null!;

    [Index(86)] public string StopzettingsReden { get; init; } = null!;

    protected bool Equals(MutationsLine other)
        => DatumModificatie.Equals(other.DatumModificatie) &&
           Ondernemingsnummer == other.Ondernemingsnummer &&
           MaatschappelijkeNaam == other.MaatschappelijkeNaam;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MutationsLine)obj);
    }

    public override int GetHashCode()
        => GetHashCodeFromField(MaatschappelijkeNaam, GetHashCodeFromField(Ondernemingsnummer, DatumModificatie.GetHashCode()));

    private static int GetHashCodeFromField(object field, int hashCode)
        => (hashCode * 397) ^ field.GetHashCode();
}
