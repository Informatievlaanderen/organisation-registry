namespace OrganisationRegistry.Body;

public class RemoveBodyMandate : BaseCommand<BodyMandateId>
{
    public BodyId BodyId { get; }
    public BodySeatId BodySeatId { get; }
    public BodyMandateId BodyMandateId => Id;

    public RemoveBodyMandate(BodyId bodyId, BodySeatId bodySeatId, BodyMandateId bodyMandateId)
    {
        BodyId = bodyId;
        BodySeatId = bodySeatId;
        Id = bodyMandateId;
    }
}
