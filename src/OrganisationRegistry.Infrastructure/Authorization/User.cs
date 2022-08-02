namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;

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
        IEnumerable<Guid> organisationIds)
    {
        Organisations = organisations.ToList();
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
        Ip = ip ?? string.Empty;
        Roles = roles;
        Bodies = bodies;
        OrganisationIds = organisationIds.ToList();
    }

    public List<string> Organisations { get; }
    public List<Guid> OrganisationIds { get; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Ip { get; set; }
    public string UserId { get; set; }
    public Role[] Roles { get; set; }
    public IEnumerable<Guid> Bodies { get; }

    public bool IsAuthorizedForVlimpersOrganisations
        => IsInAnyOf(
            Role.VlimpersBeheerder,
            Role.Developer,
            Role.AlgemeenBeheerder);

    public bool IsInAnyOf(params Role[] roles)
        => Roles.Any(roles.Contains);

    public bool IsDecentraalBeheerderForOrganisation(string ovoNumber)
        => IsInAnyOf(Role.DecentraalBeheerder) &&
           Organisations.Contains(ovoNumber);
    public bool IsDecentraalBeheerderForOrganisation(Guid organisationId)
        => IsInAnyOf(Role.DecentraalBeheerder) &&
           OrganisationIds.Contains(organisationId);

    public bool IsDecentraalBeheerderForBody(Guid bodyId)
        => IsInAnyOf(Role.DecentraalBeheerder) &&
           Bodies.Contains(bodyId);
}
