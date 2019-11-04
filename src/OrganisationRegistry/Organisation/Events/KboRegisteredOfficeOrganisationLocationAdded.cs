namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class KboRegisteredOfficeOrganisationLocationAdded : BaseEvent<KboRegisteredOfficeOrganisationLocationAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationLocationId { get; }
        public Guid LocationId { get; }
        public string LocationFormattedAddress { get; }
        public bool IsMainLocation { get; }
        public Guid? LocationTypeId { get; }
        public string LocationTypeName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public KboRegisteredOfficeOrganisationLocationAdded(
            Guid organisationId,
            Guid organisationLocationId,
            Guid locationId,
            string locationFormattedAddress,
            bool isMainLocation,
            Guid? locationTypeId,
            string locationTypeName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationLocationId = organisationLocationId;
            LocationId = locationId;
            LocationFormattedAddress = locationFormattedAddress;
            IsMainLocation = isMainLocation;
            LocationTypeId = locationTypeId;
            LocationTypeName = locationTypeName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
