namespace OrganisationRegistry.MandateRoleType.Events;

using System;
using Newtonsoft.Json;

public class MandateRoleTypeCreated : BaseEvent<MandateRoleTypeCreated>
{
    public Guid MandateRoleTypeId => Id;

    public string Name { get; }

    public MandateRoleTypeCreated(
        MandateRoleTypeId mandateRoleTypeId,
        MandateRoleTypeName name)
    {
        Id = mandateRoleTypeId;
        Name = name;
    }

    [JsonConstructor]
    public MandateRoleTypeCreated(
        Guid mandateRoleTypeId,
        string name)
        : this(
            new MandateRoleTypeId(mandateRoleTypeId),
            new MandateRoleTypeName(name)) { }
}