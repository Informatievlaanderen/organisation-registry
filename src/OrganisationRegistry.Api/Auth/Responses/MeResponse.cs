namespace OrganisationRegistry.Api.Auth.Responses;

using System.Collections.Generic;
using System.Linq;

public struct MeResponse
{
    public string Name { get; init; }
    public string Role { get; init; }
    public string[] Permissions { get; init; }


    public static MeResponse Create(string name, string role, IEnumerable<string>? permissions)
        => new()
        {
            Name = name,
            Role = role,
            Permissions = permissions?.ToArray() ?? [],
        };
}
