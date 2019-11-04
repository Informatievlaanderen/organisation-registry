namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationCapacityBecameActive: BaseEvent<OrganisationCapacityBecameActive>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationCapacityId { get; }
        public Guid CapacityId { get; }
        public Guid? PersonId { get; }
        public Guid? FunctionTypeId { get; }
        public DateTime? StartOfValidity { get; }

        public OrganisationCapacityBecameActive(
            Guid organisationId,
            Guid organisationCapacityId,
            Guid capacityId,
            Guid? personId,
            Guid? functionTypeId,
            DateTime? startOfValidity)
        {
            Id = organisationId;

            OrganisationCapacityId = organisationCapacityId;
            CapacityId = capacityId;
            PersonId = personId;
            FunctionTypeId = functionTypeId;
            StartOfValidity = startOfValidity;
        }
    }
}
