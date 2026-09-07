namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes only when the body identified by the supplied <see cref="BodyContext"/>
/// belongs to the user's own organisation or a child organisation (i.e. it is
/// present in <see cref="IUser.Bodies"/>). Requires both <see cref="UserContext"/>
/// and <see cref="BodyContext"/> and fails closed when either is missing.
/// </summary>
public sealed class DecentraalBodyRestriction : IRestriction
{
    public static readonly DecentraalBodyRestriction Instance = new();

    private DecentraalBodyRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<UserContext>().FirstOrDefault() is { } userContext &&
           contexts.OfType<BodyContext>().FirstOrDefault() is { } bodyContext &&
           userContext.User.Bodies.Contains(bodyContext.BodyId);

    public override string ToString() => "DecentraalBody";
}
