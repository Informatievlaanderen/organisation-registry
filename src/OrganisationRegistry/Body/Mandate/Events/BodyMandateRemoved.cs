namespace OrganisationRegistry.Body.Events;

using System;

public class BodyMandateRemoved : BaseEvent<BodyMandateRemoved>
{
    public Guid BodyId { get; }
    public Guid BodySeatId { get; }
    public Guid BodyMandateId { get; }

    public BodyMandateRemoved(Guid bodyId, Guid bodySeatId, Guid bodyMandateId)
    {
        Id = bodyMandateId;
        BodyId = bodyId;
        BodySeatId = bodySeatId;
        BodyMandateId = bodyMandateId;
    }
}
