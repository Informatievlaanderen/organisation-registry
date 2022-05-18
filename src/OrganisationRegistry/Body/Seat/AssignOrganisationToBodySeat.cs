namespace OrganisationRegistry.Body;

using Organisation;

public class AssignOrganisationToBodySeat : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodySeatId BodySeatId { get; }
    public BodyMandateId BodyMandateId { get; }
    public OrganisationId OrganisationId { get; }
    public Period Validity { get; }

    public AssignOrganisationToBodySeat(
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
