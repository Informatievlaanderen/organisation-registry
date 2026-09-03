namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// A restriction decides whether a set of <see cref="IRestrictionContext"/> is
/// allowed. Restrictions are stored non-generically inside a
/// <see cref="PermissionEntry"/> and evaluated by the
/// <see cref="PermissionSet.IsSatisfiedFor"/> engine.
///
/// Composition follows a simple algebra: an AND of restrictions lives inside a
/// single grant (see <see cref="CompositeAndRestriction"/>); OR across grants is
/// the union of a <see cref="PermissionSet"/>. A restriction must fail closed
/// when it receives a context type it does not understand.
///
/// Multiple contexts can be supplied so that a restriction can mix resource-level
/// context (e.g. <see cref="KeyContext"/>) with user-level context
/// (<see cref="UserContext"/>).
/// </summary>
public interface IRestriction
{
    bool IsOkWith(params IRestrictionContext[] contexts);
}
