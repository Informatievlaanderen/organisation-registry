namespace OrganisationRegistry.Body
{
    using System;
    using System.Collections.Generic;
    using Organisation;
    using Person;

    public class OrganisationBodyMandate : BodyMandate
    {
        public OrganisationId OrganisationId { get; }
        public string OrganisationName { get; }

        public OrganisationBodyMandate(
            BodyMandateId bodyMandateId,
            OrganisationId organisationId,
            string organisationName,
            Period validity)
            : base(bodyMandateId, validity)
        {
            OrganisationId = organisationId;
            OrganisationName = organisationName;
        }

        public override void AssignPerson(
            DelegationAssignmentId delegationAssignmentId,
            PersonId personId,
            string personFullName,
            Dictionary<Guid, string> contacts,
            Period period)
        {
            AddAssignment(
                new Assignment(
                    delegationAssignmentId,
                    personId,
                    personFullName,
                    contacts,
                    period));
        }
    }
}
