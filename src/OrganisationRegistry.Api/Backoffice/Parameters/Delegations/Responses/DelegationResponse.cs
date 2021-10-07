namespace OrganisationRegistry.Api.Backoffice.Parameters.Delegations.Responses
{
    using System;
    using System.Runtime.Serialization;
    using SqlServer.Delegations;

    public class DelegationResponse
    {
        public Guid Id { get; set; }

        public Guid BodyId { get; set; }
        public string BodyName { get; set; }
        public string BodyOrganisationName { get; set; }
        public string OrganisationName { get; set; }
        public string FunctionTypeName { get; set; }
        public Guid BodySeatId { get; set; }
        public string BodySeatNumber { get; set; }
        public string BodySeatName { get; set; }
        public bool IsDelegated { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public DelegationResponse(DelegationListItem projectionItem)
        {
            Id = projectionItem.Id;

            BodyId = projectionItem.BodyId;
            BodyName = projectionItem.BodyName;
            BodyOrganisationName = projectionItem.BodyOrganisationName;
            OrganisationName = projectionItem.OrganisationName;
            FunctionTypeName = projectionItem.FunctionTypeName;
            BodySeatId = projectionItem.BodySeatId;
            BodySeatNumber = projectionItem.BodySeatNumber;
            BodySeatName = projectionItem.BodySeatName;
            ValidFrom = projectionItem.ValidFrom;
            ValidTo = projectionItem.ValidTo;
            IsDelegated = projectionItem.IsDelegated;
        }
    }
}
