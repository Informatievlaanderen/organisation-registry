namespace OrganisationRegistry.Location
{
    using System;

    public class LocationId : GenericId<LocationId>
    {
        public LocationId(Guid id) : base(id) { }
    }
}
