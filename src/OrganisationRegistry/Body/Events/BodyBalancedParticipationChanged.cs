namespace OrganisationRegistry.Body.Events
{
    using System;
    using Newtonsoft.Json;

    public class BodyBalancedParticipationChanged : BaseEvent<BodyBalancedParticipationChanged>
    {
        public Guid BodyId => Id;

        public bool BalancedParticipationObligatory { get; }
        public bool PreviousBalancedParticipationObligatory { get; }

        public string BalancedParticipationExtraRemark { get; }
        public string PreviousBalancedParticipationExtraRemark { get; }

        public string BalancedParticipationExceptionMeasure { get; }
        public string PreviousBalancedParticipationExceptionMeasure { get; }

        [JsonConstructor]
        public BodyBalancedParticipationChanged(
            Guid bodyId,
            bool balancedParticipationObligatory,
            string balancedParticipationExtraRemark,
            string balancedParticipationExceptionMeasure,
            bool previousBalancedParticipationObligatory,
            string previousBalancedParticipationExtraRemark,
            string previousBalancedParticipationExceptionMeasure)
        {
            Id = bodyId;

            BalancedParticipationObligatory = balancedParticipationObligatory;
            BalancedParticipationExtraRemark = balancedParticipationExtraRemark;
            BalancedParticipationExceptionMeasure = balancedParticipationExceptionMeasure;

            PreviousBalancedParticipationObligatory = previousBalancedParticipationObligatory;
            PreviousBalancedParticipationExtraRemark = previousBalancedParticipationExtraRemark;
            PreviousBalancedParticipationExceptionMeasure = previousBalancedParticipationExceptionMeasure;
        }
    }
}
