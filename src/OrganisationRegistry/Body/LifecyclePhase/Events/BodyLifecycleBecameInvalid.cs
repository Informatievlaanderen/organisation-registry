namespace OrganisationRegistry.Body.Events;

using System;

public class BodyLifecycleBecameInvalid : BaseEvent<BodyLifecycleBecameInvalid>
{
    public Guid BodyId => Id;

    public BodyLifecycleBecameInvalid(Guid bodyId)
    {
        Id = bodyId;
    }
}