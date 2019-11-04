namespace OrganisationRegistry.Body
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class DelegationAssignmentId : GuidValueObject<DelegationAssignmentId>
    {
        public DelegationAssignmentId([JsonProperty("id")] Guid delegationAssignmentId) : base(delegationAssignmentId) { }
    }
}

