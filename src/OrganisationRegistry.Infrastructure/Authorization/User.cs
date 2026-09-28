namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;

public class User : IUser
{
    public User(
        string firstName,
        string lastName,
        string userId,
        string? ip,
        Role[] roles,
        IEnumerable<string> organisations,
        IEnumerable<Guid> bodies,
        IEnumerable<Guid> organisationIds,
        PermissionSet? permissions = null)
    {
        Organisations = organisations.ToList();
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
        Ip = ip ?? string.Empty;
        Roles = roles;
        Bodies = bodies;
        OrganisationIds = organisationIds.ToList();
        Permissions = permissions ?? RolePermissionMap.For(roles);
    }

    public User(
        string firstName,
        string lastName,
        string userId,
        string? ip,
        IEnumerable<string> organisations,
        IEnumerable<Guid> bodies,
        IEnumerable<Guid> organisationIds,
        PermissionSet permissions)
    {
        Organisations = organisations.ToList();
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
        Ip = ip ?? string.Empty;
        Roles = Array.Empty<Role>();
        Bodies = bodies;
        OrganisationIds = organisationIds.ToList();
        Permissions = permissions;
    }

    public List<string> Organisations { get; }
    public List<Guid> OrganisationIds { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Ip { get; set; }
    public string UserId { get; set; }
    public Role[] Roles { get; set; }
    public IEnumerable<Guid> Bodies { get; }
    public PermissionSet Permissions { get; }

    public bool HasPermission(Permission permission)
        => Permissions.Contains(permission);

    public bool HasAnyPermission(params Permission[] permissions)
        => permissions.Any(Permissions.Contains);

    public bool IsSatisfiedFor(Permission permission, params IRestrictionContext[] contexts)
        => Permissions.IsSatisfiedFor(permission, contexts);
}
