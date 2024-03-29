namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Generic;

public interface IUser
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string UserId { get; set; }
    string Ip { get; set; }
    Role[] Roles { get; set; }
    bool IsAuthorizedForVlimpersOrganisations { get; }
    List<string> Organisations { get; }
    bool IsInAnyOf(params Role[] roles);
    bool IsDecentraalBeheerderForOrganisation(string ovoNumber);
    bool IsDecentraalBeheerderForOrganisation(Guid organisationId);
    bool IsDecentraalBeheerderForBody(Guid bodyId);
}
