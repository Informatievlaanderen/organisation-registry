namespace OrganisationRegistry.Location.Events;

using System;

public interface IHasLocation
{
    public Guid LocationId { get; }

    public string? CrabLocationId { get;}

    public string FormattedAddress { get;}
    public string Street { get;}
    public string ZipCode { get;}
    public string City { get;}
    public string Country { get;}
}
