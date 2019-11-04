namespace OrganisationRegistry.Body
{
    using System;
    using System.Collections.Generic;
    using Function;
    using Organisation;
    using Person;

    public class FunctionTypeBodyMandate : BodyMandate
    {
        public OrganisationId OrganisationId { get; }
        public string OrganisationName { get; }

        public FunctionTypeId FunctionTypeId { get; }
        public string FunctionTypeName { get; }

        public FunctionTypeBodyMandate(
            BodyMandateId bodyMandateId,
            OrganisationId organisationId,
            string organisationName,
            FunctionTypeId functionTypeIdTypeId,
            string functionTypeName,
            Period validity)
            : base(bodyMandateId, validity)
        {
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            FunctionTypeId = functionTypeIdTypeId;
            FunctionTypeName = functionTypeName;
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
