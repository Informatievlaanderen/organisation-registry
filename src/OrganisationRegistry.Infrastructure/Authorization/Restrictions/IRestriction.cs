namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Non-generic marker for restriction values so <see cref="PermissionEntry"/>
/// can store them without leaking generics. Concrete restrictions always
/// implement the typed <see cref="IRestriction{TContext}"/> variant.
/// </summary>
public interface IRestriction
{
    string Domain { get; }
}

/// <summary>
/// A restriction that can decide whether a specific context is allowed.
/// Contravariant on TContext so derived-context readers stay compatible.
/// </summary>
public interface IRestriction<in TContext> : IRestriction
    where TContext : IRestrictionContext<TContext>
{
    bool IsOkWith(TContext context);
}
