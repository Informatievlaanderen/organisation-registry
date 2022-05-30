namespace OrganisationRegistry.MandateRoleType.Events;

using System;
using Newtonsoft.Json;

public class MandateRoleTypeUpdated : BaseEvent<MandateRoleTypeUpdated>
{
    public Guid MandateRoleTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public MandateRoleTypeUpdated(
        MandateRoleTypeId mandateRoleTypeId,
        MandateRoleTypeName name,
        MandateRoleTypeName previousName)
    {
        Id = mandateRoleTypeId;
        Name = name;
        PreviousName = previousName;
    }

    [JsonConstructor]
    public MandateRoleTypeUpdated(
        Guid mandateRoleTypeId,
        string name,
        string previousName)
        : this(
            new MandateRoleTypeId(mandateRoleTypeId),
            new MandateRoleTypeName(name),
            new MandateRoleTypeName(previousName)) { }
}