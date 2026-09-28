namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;

public class SecurityInformation
{
    public SecurityInformation(string userName) : this(
        userName,
        new List<Role>(),
        new List<string>(),
        new List<Guid>(),
        new List<Guid>(),
        PermissionSet.Empty)
    {
    }

    public SecurityInformation(
        string userName,
        IList<Role> roles,
        IList<string> ovoNumbers,
        IList<Guid> organisationIds,
        IList<Guid> bodyIds)
        : this(userName, roles, ovoNumbers, organisationIds, bodyIds, PermissionSet.Empty)
    {
    }

    public SecurityInformation(
        string userName,
        IList<Role> roles,
        IList<string> ovoNumbers,
        IList<Guid> organisationIds,
        IList<Guid> bodyIds,
        PermissionSet permissions)
    {
        UserName = userName;
        Roles = roles;
        OvoNumbers = ovoNumbers;
        OrganisationIds = organisationIds;
        BodyIds = bodyIds;
        Permissions = permissions;
    }

    public string UserName { get; }

    public IList<Role> Roles { get; }

    public IList<string> OvoNumbers { get; }

    public IList<Guid> OrganisationIds { get; }

    public IList<Guid> BodyIds { get; }

    public PermissionSet Permissions { get; }

    public static SecurityInformation None()
        => new(string.Empty);
}
