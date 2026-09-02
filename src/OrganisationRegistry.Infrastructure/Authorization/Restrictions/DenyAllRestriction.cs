namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

/// <summary>
/// Never allows. Returned by <see cref="PermissionSet.GetRestriction{TContext}"/>
/// when a permission was not granted with any restriction for the requested
/// context (fail-closed).
/// </summary>
public sealed class DenyAllRestriction<TContext> : IRestriction<TContext>
    where TContext : IRestrictionContext<TContext>
{
    public static readonly DenyAllRestriction<TContext> Instance = new();

    private DenyAllRestriction() { }

    public string Domain => TContext.Domain;

    public bool IsOkWith(TContext context) => false;

    public override string ToString() => $"DenyAll<{typeof(TContext).Name}>";
}
