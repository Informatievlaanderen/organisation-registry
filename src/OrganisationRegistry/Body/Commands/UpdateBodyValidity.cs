namespace OrganisationRegistry.Body.Commands
{
    public class UpdateBodyValidity : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public Period FormalValidity { get; }

        public UpdateBodyValidity(
            BodyId bodyId,
            Period formalValidity)
        {
            Id = bodyId;

            FormalValidity = formalValidity;
        }
    }
}
