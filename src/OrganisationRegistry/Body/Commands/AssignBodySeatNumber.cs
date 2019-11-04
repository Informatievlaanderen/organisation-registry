namespace OrganisationRegistry.Body.Commands
{
    public class AssignBodySeatNumber : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }

        public AssignBodySeatNumber(
            BodyId bodyId,
            BodySeatId bodySeatId)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
        }
    }
}
