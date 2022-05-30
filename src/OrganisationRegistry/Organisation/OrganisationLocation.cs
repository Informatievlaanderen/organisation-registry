namespace OrganisationRegistry.Organisation;

using System;

public class OrganisationLocation : IOrganisationField, IValidityBuilder<OrganisationLocation>
{
    public Guid Id => OrganisationLocationId;
    public Guid OrganisationLocationId { get; }
    public Guid OrganisationId { get; }
    public Guid LocationId { get; }
    public string FormattedAddress { get; }
    public bool IsMainLocation { get; set; }
    public Guid? LocationTypeId { get; }
    public string? LocationTypeName { get; }
    public Period Validity { get; }
    public Source Source { get; }

    public OrganisationLocation(
        Guid organisationLocationId,
        Guid organisationId,
        Guid locationId,
        string formattedAddress,
        bool isMainLocation,
        Guid? locationTypeId,
        string? locationTypeName,
        Period validity,
        Source source)
    {
        OrganisationLocationId = organisationLocationId;
        OrganisationId = organisationId;
        LocationId = locationId;
        IsMainLocation = isMainLocation;
        LocationTypeId = locationTypeId;
        LocationTypeName = locationTypeName;
        Validity = validity;
        Source = source;
        FormattedAddress = formattedAddress;
    }

    public bool IsValid(DateTime date)
        => Validity.OverlapsWith(new Period(new ValidFrom(date), new ValidTo(date)));

    protected bool Equals(OrganisationLocation other)
        => OrganisationLocationId.Equals(other.OrganisationLocationId);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((OrganisationLocation)obj);
    }

    public override int GetHashCode()
        => OrganisationLocationId.GetHashCode();

    public OrganisationLocation WithValidity(Period period)
        => new(
            OrganisationLocationId,
            OrganisationId,
            LocationId,
            FormattedAddress,
            IsMainLocation,
            LocationTypeId,
            LocationTypeName,
            period,
            Source);

    public OrganisationLocation WithValidFrom(ValidFrom validFrom)
        => WithValidity(new Period(validFrom, Validity.End));

    public OrganisationLocation WithValidTo(ValidTo validTo)
        => WithValidity(new Period(Validity.Start, validTo));
}