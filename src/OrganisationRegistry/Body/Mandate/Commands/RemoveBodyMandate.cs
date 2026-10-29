namespace OrganisationRegistry.Body;

public class RemoveBodyMandate : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;
    public BodySeatId BodySeatId { get; }
    public BodyMandateId BodyMandateId { get; }

    public RemoveBodyMandate(BodyId bodyId, BodySeatId bodySeatId, BodyMandateId bodyMandateId)
    {
        Id = bodyId;
        BodySeatId = bodySeatId;
        BodyMandateId = bodyMandateId;
    }
}
