namespace OrganisationRegistry.Infrastructure.Authorization.Restrictions;

using System.Linq;

/// <summary>
/// Passes only when the target organisation is <em>not</em> under Vlimpers
/// management. Requires the context to carry the
/// <see cref="IVlimpersManagedContext"/> capability and fails closed for any
/// context that does not (so the absence of the flag is never read as
/// "not under Vlimpers management").
///
/// The mirror image of <see cref="RequireUnderVlimpersManagementRestriction"/>;
/// used to keep Vlimpers-managed organisations out of a DecentraalBeheerder's
/// reach. Stateless: use the shared <see cref="Instance"/>.
/// </summary>
public sealed class RequireNotUnderVlimpersManagementRestriction : IRestriction
{
    public static readonly RequireNotUnderVlimpersManagementRestriction Instance = new();

    private RequireNotUnderVlimpersManagementRestriction() { }

    public bool IsOkWith(params IRestrictionContext[] contexts)
        => contexts.OfType<IVlimpersManagedContext>().FirstOrDefault() is { } vlimpers &&
           !vlimpers.IsUnderVlimpersManagement;

    public override string ToString() => "RequireNotUnderVlimpersManagement";
}
