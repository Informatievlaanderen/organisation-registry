namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;

public static class PolicyExtensions
{
    public static void ThrowOnViolation<TPolicy>(this TPolicy policy, IUser user)
        where TPolicy : ISecurityPolicy
    {
        var policyResult = policy.Check(user);
        if (policyResult.Exception is { } exception)
            throw exception;
    }
}
