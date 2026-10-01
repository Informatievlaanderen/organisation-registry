namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes when the body identified by the supplied <see cref="BodyContext"/>
/// belongs to the user's own organisation or a child organisation. This is
/// checked two ways, either of which suffices:
/// <list type="bullet">
/// <item>the body is present in the (cached) <see cref="IUser.Bodies"/> list, or</item>
/// <item>the context's (live, read straight off the aggregate) <see cref="BodyContext.OrganisationId"/>
/// is one of the user's own/child organisations.</item>
/// </list>
/// The second check exists so access isn't solely gated by <see cref="IUser.Bodies"/>,
/// which is computed from a read model and only refreshed when the security
/// cache is invalidated — e.g. right after a body is newly (re)linked to an
/// organisation, before that read model/cache has caught up.
/// Requires both <see cref="UserContext"/> and <see cref="BodyContext"/> and
/// fails closed when either is missing.
/// </summary>
public sealed class DecentraalBodyRestriction : IRestriction
{
    public static readonly DecentraalBodyRestriction Instance = new();

    private DecentraalBodyRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<UserContext>().FirstOrDefault() is { } userContext &&
           contexts.OfType<BodyContext>().FirstOrDefault() is { } bodyContext &&
           (userContext.User.Bodies.Contains(bodyContext.BodyId) ||
            (bodyContext.OrganisationId is { } organisationId &&
             userContext.User.OrganisationIds.Contains(organisationId)));

    public override string ToString() => "DecentraalBody";
}
