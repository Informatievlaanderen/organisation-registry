namespace OrganisationRegistry.Body.Events;

using System;

public class BodyLifecycleBecameValid : BaseEvent<BodyLifecycleBecameValid>
{
    public Guid BodyId => Id;

    public BodyLifecycleBecameValid(Guid bodyId)
    {
        Id = bodyId;
    }
}
