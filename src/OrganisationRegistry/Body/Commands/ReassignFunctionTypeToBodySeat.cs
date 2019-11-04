namespace OrganisationRegistry.Body.Commands
{
    using Function;
    using Organisation;

    public class ReassignFunctionTypeToBodySeat : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }
        public BodyMandateId BodyMandateId { get; }
        public OrganisationId OrganisationId { get; }
        public FunctionTypeId FunctionTypeId { get; }
        public Period Validity { get; }

        public ReassignFunctionTypeToBodySeat(
            BodyId bodyId,
            BodyMandateId bodyMandateId,
            BodySeatId bodySeatId,
            OrganisationId organisationId,
            FunctionTypeId functionTypeId,
            Period validity)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            BodyMandateId = bodyMandateId;
            OrganisationId = organisationId;
            FunctionTypeId = functionTypeId;
            Validity = validity;
        }
    }
}
