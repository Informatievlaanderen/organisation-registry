namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Always allows. Used when a user has an unrestricted grant for a permission
/// that another role only granted with restrictions — the unrestricted grant
/// absorbs the restricted ones per union semantics.
/// </summary>
public sealed class UnrestrictedRestriction<TContext> : IRestriction<TContext>
    where TContext : IRestrictionContext<TContext>
{
    public static readonly UnrestrictedRestriction<TContext> Instance = new();

    private UnrestrictedRestriction() { }

    public string Domain => TContext.Domain;

    public bool IsOkWith(TContext context) => true;

    public override string ToString() => $"Unrestricted<{typeof(TContext).Name}>";
}
