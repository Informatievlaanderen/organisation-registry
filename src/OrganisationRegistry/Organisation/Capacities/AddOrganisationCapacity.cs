namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;
    using Capacity;
    using ContactType;
    using Function;
    using Person;
    using Location;

    public class AddOrganisationCapacity : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationCapacityId { get; }
        public CapacityId CapacityId { get; }
        public PersonId? PersonId { get; }
        public FunctionTypeId? FunctionId { get; }
        public LocationId? LocationId { get; }
        public Dictionary<ContactTypeId, string> Contacts { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationCapacity(
            Guid organisationCapacityId,
            OrganisationId organisationId,
            CapacityId capacityId,
            PersonId? personId,
            FunctionTypeId? functionId,
            LocationId? locationId,
            Dictionary<ContactTypeId, string>? contacts,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationCapacityId = organisationCapacityId;
            CapacityId = capacityId;
            PersonId = personId;
            FunctionId = functionId;
            LocationId = locationId;
            Contacts = contacts ?? new Dictionary<ContactTypeId, string>();
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
