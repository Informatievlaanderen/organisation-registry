namespace OrganisationRegistry.Api.Backoffice.Body.Responses
{
    using System;
    using System.Runtime.Serialization;
    using SqlServer.Body;

    [DataContract]
    public class BodyBalancedParticipationResponse
    {
        public Guid Id { get; }

        public bool? Obligatory { get; }
        public string? ExtraRemark { get; }
        public string? ExceptionMeasure { get; }

        public BodyBalancedParticipationResponse(BodyDetail projectionItem)
        {
            Id = projectionItem.Id;

            Obligatory = projectionItem.IsBalancedParticipationObligatory;
            ExtraRemark = projectionItem.BalancedParticipationExtraRemark;
            ExceptionMeasure = projectionItem.BalancedParticipationExceptionMeasure;
        }
    }
}
