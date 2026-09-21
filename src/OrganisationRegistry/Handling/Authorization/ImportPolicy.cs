namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Collections.Generic;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Infrastructure.Domain;
using Organisation;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for imports (CSV-based organisation create/terminate
/// processing). Access is driven entirely by the <see cref="Permission.CanImport"/>
/// permission and its (optional) restriction, evaluated per target organisation against
/// its OVO number and Vlimpers-management flag.
///
/// A holder of an unrestricted <c>CanImport</c> grant (AlgemeenBeheerder, Developer)
/// always passes; a VlimpersBeheerder (restricted grant) only passes when every target
/// organisation is under Vlimpers management. This mirrors the behaviour of
/// <see cref="ChildPolicy"/> for the same Vlimpers-management restriction.
/// </summary>
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
        foreach (var organisationId in _organisationIds)
        {
            var organisation = _session.Get<Organisation>(organisationId);

            var isSatisfied = user.IsSatisfiedFor(
                Permission.CanImport,
                new UserContext(user),
                new OrganisationContext(organisation.State.OvoNumber),
                new VlimpersManagementContext(organisation.State.UnderVlimpersManagement));

            if (!isSatisfied)
                return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
        }

        return AuthorizationResult.Success();
    }

    public override string ToString()
        => "Geen machtiging op deze organisatie.";
}
