namespace OrganisationRegistry.Body.Commands
{
    public class UpdateBodyBalancedParticipation : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public bool BalancedParticipationObligatory { get; }
        public string BalancedParticipationExtraRemark { get; }
        public string BalancedParticipationExceptionMeasure { get; }

        public UpdateBodyBalancedParticipation(
            BodyId bodyId,
            bool balancedParticipationObligatory,
            string balancedParticipationExtraRemark,
            string balancedParticipationExceptionMeasure)
        {
            Id = bodyId;

            BalancedParticipationObligatory = balancedParticipationObligatory;
            BalancedParticipationExtraRemark = balancedParticipationExtraRemark;
            BalancedParticipationExceptionMeasure = balancedParticipationExceptionMeasure;
        }
    }
}
