namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes only when the user is a DecentraalBeheerder for the organisation
/// identified by the supplied <see cref="OrganisationContext"/> (by OVO number
/// or by organisation id). Requires both <see cref="UserContext"/> and
/// <see cref="OrganisationContext"/> and fails closed when either is missing
/// or when the context carries no identifier.
/// </summary>
public sealed class DecentraalOrganisationRestriction : IRestriction
{
    public static readonly DecentraalOrganisationRestriction Instance = new();

    private DecentraalOrganisationRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<UserContext>().FirstOrDefault() is { } userContext &&
           contexts.OfType<OrganisationContext>().FirstOrDefault() is { } organisationContext &&
           organisationContext switch
           {
               OrganisationContext.ByOvoNumber { OvoNumber: { } ovoNumber }
                   => userContext.User.Organisations.Contains(ovoNumber),

               OrganisationContext.ById { OrganisationId: { } organisationId }
                   => userContext.User.OrganisationIds.Contains(organisationId),

               _ => false,
           };

    public override string ToString() => "DecentraalOrganisation";
}
