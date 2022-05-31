namespace OrganisationRegistry.Body.Events;

using System;

public class BodyNumberAssigned : BaseEvent<BodyNumberAssigned>
{
    public Guid BodyId => Id;

    public string BodyNumber { get; }

    public BodyNumberAssigned(
        Guid bodyId,
        string bodyNumber)
    {
        Id = bodyId;

        BodyNumber = bodyNumber;
    }
}
