namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using Location;
    using LocationType;

    public class AddOrganisationLocation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationLocationId { get; }
        public LocationId LocationId { get; }
        public bool IsMainLocation { get; }
        public LocationTypeId LocationTypeId { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationLocation(
            Guid organisationLocationId,
            OrganisationId organisationId,
            LocationId locationId,
            bool isMainLocation,
            LocationTypeId? locationTypeId,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationLocationId = organisationLocationId;
            LocationId = locationId;
            IsMainLocation = isMainLocation;
            LocationTypeId = locationTypeId;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
