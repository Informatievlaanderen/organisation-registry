namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes only when the user is a DecentraalBeheerder for the organisation
/// identified by the supplied <see cref="OrganisationContext"/>. Requires both
/// <see cref="UserContext"/> and <see cref="OrganisationContext"/> and fails
/// closed when either is missing.
/// </summary>
public sealed class DecentraalOrganisationRestriction : IRestriction
{
    public static readonly DecentraalOrganisationRestriction Instance = new();

    private DecentraalOrganisationRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<UserContext>().FirstOrDefault() is { } userContext &&
           contexts.OfType<OrganisationContext>().FirstOrDefault() is { } organisationContext &&
           userContext.User.Organisations.Contains(organisationContext.OvoNumber);

    public override string ToString() => "DecentraalOrganisation";
}
