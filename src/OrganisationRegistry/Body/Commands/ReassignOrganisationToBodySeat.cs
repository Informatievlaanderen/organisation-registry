namespace OrganisationRegistry.Body.Commands
{
    using Organisation;

    public class ReassignOrganisationToBodySeat : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }
        public BodyMandateId BodyMandateId { get; }
        public OrganisationId OrganisationId { get; }
        public Period Validity { get; }

        public ReassignOrganisationToBodySeat(
            BodyId bodyId,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            OrganisationId organisationId,
            Period validity)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            BodyMandateId = bodyMandateId;
            OrganisationId = organisationId;
            Validity = validity;
        }
    }
}
