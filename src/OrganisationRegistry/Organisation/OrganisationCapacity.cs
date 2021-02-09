namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;

    public class OrganisationCapacity : IOrganisationField, IValidityBuilder<OrganisationCapacity>
    {
        public Guid Id => OrganisationCapacityId;
        public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
        public Guid OrganisationCapacityId { get; }
        public Guid CapacityId { get; }
        public string CapacityName { get; }
        public Guid? PersonId { get; }
        public string PersonName { get; }
        public Guid? FunctionTypeId { get; }
        public string FunctionTypeName { get; }
        public Guid? LocationId { get; }
        public string LocationName { get; }
        public Dictionary<Guid, string> Contacts { get; }
        public Period Validity { get; }
        public bool IsActive { get; set; } // TODO: would Activated be a more appropriate name? Also, how about immutability?

        public OrganisationCapacity(
            Guid organisationCapacityId,
            Guid organisationId,
            Guid capacityId,
            string capacityName,
            Guid? personId,
            string personName,
            Guid? functionTypeId,
            string functionTypeName,
            Guid? locationId,
            string locationName,
            Dictionary<Guid, string> contacts,
            Period validity,
            bool isActive)
        {
            OrganisationId = organisationId;
            OrganisationCapacityId = organisationCapacityId;
            CapacityId = capacityId;
            PersonId = personId;
            FunctionTypeId = functionTypeId;
            LocationId = locationId;
            Contacts = contacts;
            Validity = validity;
            CapacityName = capacityName;
            PersonName = personName;
            FunctionTypeName = functionTypeName;
            LocationName = locationName;
            IsActive = isActive;
        }

        public bool ShouldBecomeActive(DateTime date)
        {
            return !IsActive && Validity.OverlapsWith(date);
        }

        public bool ShouldBecomeInactive(DateTime date)
        {
            return IsActive && !Validity.OverlapsWith(date);
        }

        public OrganisationCapacity WithValidity(Period period)
        {
            return new OrganisationCapacity(
                OrganisationCapacityId,
                OrganisationId,
                CapacityId,
                CapacityName,
                PersonId,
                PersonName,
                FunctionTypeId,
                FunctionTypeName,
                LocationId,
                LocationName,
                Contacts,
                period,
                IsActive);
        }

        public OrganisationCapacity WithValidFrom(ValidFrom validFrom)
        {
            return WithValidity(new Period(validFrom, Validity.End));
        }

        public OrganisationCapacity WithValidTo(ValidTo validTo)
        {
            return WithValidity(new Period(Validity.Start, validTo));
        }
    }
}
