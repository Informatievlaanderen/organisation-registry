namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Collections.Generic;
using Infrastructure.Authorization;
using Infrastructure.Domain;
using Organisation;
using Organisation.Exceptions;

public class ImportPolicy : ISecurityPolicy
{
    private readonly ISession _session;
    private readonly IEnumerable<Guid> _organisationIds;

    public ImportPolicy(ISession session, params Guid[] organisationIds)
    {
        _session = session;
        _organisationIds = organisationIds;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        if (user.IsInAnyOf(Role.VlimpersBeheerder))
            return CheckVlimpers(user);

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    private AuthorizationResult CheckVlimpers(IUser user)
    {
        foreach (var organisationId in _organisationIds)
        {
            var organisation = _session.Get<Organisation>(organisationId);

            var vlimpersPolicy = new VlimpersPolicy(organisation.State.UnderVlimpersManagement, organisation.State.OvoNumber);
            var authorizationResult = vlimpersPolicy.Check(user);
            if (!authorizationResult.IsSuccessful)
                return authorizationResult;
        }

        return AuthorizationResult.Success();
    }

    public override string ToString()
        => "Geen machtiging op deze organisatie.";
}
