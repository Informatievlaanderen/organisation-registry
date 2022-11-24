namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;

public class OrganisationLocations : OrganisationList<OrganisationLocation>
{
    public OrganisationLocations() { }

    public OrganisationLocations(IEnumerable<OrganisationLocation> organisationLocations) : base(organisationLocations) { }

    public OrganisationLocations(params OrganisationLocation[] organisationLocations) : base(organisationLocations) { }

    public bool AlreadyHasTheSameOrganisationAndLocationInTheSamePeriod(OrganisationLocation organisationLocation)
        => Except(organisationLocation.OrganisationLocationId)
            .WithLocation(organisationLocation.LocationId)
            .OverlappingWith(organisationLocation.Validity)
            .Any();

    public bool OrganisationAlreadyHasAMainLocationInTheSamePeriod(OrganisationLocation organisationLocation, OrganisationLocation? maybeKboRegisteredOffice)
    {
        var locationsToCheck = new OrganisationLocations(this);

        if (maybeKboRegisteredOffice is { } kboRegisteredOffice)
            locationsToCheck.Add(kboRegisteredOffice);

        return locationsToCheck
            .Except(organisationLocation.OrganisationLocationId)
            .OnlyMainLocations()
            .OverlappingWith(organisationLocation.Validity)
            .Any();
    }

    public OrganisationLocations Except(Guid organisationLocationId)
        => new(
            this.Where(ob => ob.OrganisationLocationId != organisationLocationId));

    public OrganisationLocations WithLocation(Guid locationId)
        => new(
            this.Where(ob => ob.LocationId == locationId));

    public OrganisationLocations OverlappingWith(Period validity)
        => new(
            this.Where(ob => ob.Validity.OverlapsWith(validity)));

    public OrganisationLocations OnlyMainLocations()
        => new(
            this.Where(ob => ob.IsMainLocation));

    public OrganisationLocation? TryFindMainOrganisationLocationValidFor(DateTime date, Guid locationId)
        => this
            .Where(location => location.LocationId == locationId)
            .Where(location => location.IsMainLocation)
            .SingleOrDefault(location => location.IsValid(date));

    public OrganisationLocation? TryFindMainOrganisationLocationValidFor(DateTime date)
        => this
            .Where(location => location.IsMainLocation)
            .SingleOrDefault(location => location.IsValid(date));
}
