namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Responses
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SqlServer.DelegationAssignments;

    public class DelegationAssignmentResponse
    {
        public Guid DelegationAssignmentId { get; set; }
        public Guid BodyId { get; set; }
        public Guid BodySeatId { get; set; }

        public Guid BodyMandateId { get; set; }

        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }

        public Dictionary<Guid, string>? Contacts { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public DelegationAssignmentResponse(DelegationAssignmentListItem delegationAssignment)
        {
            DelegationAssignmentId = delegationAssignment.Id;
            BodyId = delegationAssignment.BodyId;
            BodySeatId = delegationAssignment.BodySeatId;

            BodyMandateId = delegationAssignment.BodyMandateId;

            PersonId = delegationAssignment.PersonId;
            PersonName = delegationAssignment.PersonName;

            Contacts = string.IsNullOrWhiteSpace(delegationAssignment.ContactsJson)
                ? null
                : JsonConvert.DeserializeObject<Dictionary<Guid, string>>(delegationAssignment.ContactsJson);

            ValidFrom = delegationAssignment.ValidFrom;
            ValidTo = delegationAssignment.ValidTo;
        }
    }
}
