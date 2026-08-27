namespace OrganisationRegistry.Acl.Internals;
using System.Collections.Generic;

public readonly record struct Capability(string Name, Tags Area);

public sealed class Registry(Dictionary<string, Capability> mapping)
{
    public Capability Resolve(string key)
        => mapping.TryGetValue(key, out var capability)
            ? capability
            : throw new KeyNotFoundException($"No capability found in ACL registry for key '{key}'.");
}
