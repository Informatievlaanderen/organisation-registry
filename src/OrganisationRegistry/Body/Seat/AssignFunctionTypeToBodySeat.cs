namespace OrganisationRegistry.Body;

using Function;
using Organisation;

public class AssignFunctionTypeToBodySeat : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodySeatId BodySeatId { get; }
    public BodyMandateId BodyMandateId { get; }
    public OrganisationId OrganisationId { get; }
    public FunctionTypeId FunctionTypeId { get; }
    public Period Validity { get; }

    public AssignFunctionTypeToBodySeat(
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
