namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes only when the target organisation is under Vlimpers management. Requires
/// the context to carry the <see cref="IVlimpersManagedContext"/> capability and
/// fails closed for any context that does not, or that reports the organisation is
/// not under Vlimpers management.
///
/// Stateless: use the shared <see cref="Instance"/>.
/// </summary>
public sealed class RequireUnderVlimpersManagementRestriction : IRestriction
{
    public static readonly RequireUnderVlimpersManagementRestriction Instance = new();

    private RequireUnderVlimpersManagementRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<IVlimpersManagedContext>().FirstOrDefault() is { } vlimpers &&
           vlimpers.IsUnderVlimpersManagement;

    public override string ToString() => "RequireUnderVlimpersManagement";
}
