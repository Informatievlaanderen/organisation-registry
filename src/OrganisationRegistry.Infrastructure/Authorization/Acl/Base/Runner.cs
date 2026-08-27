using OrganisationRegistry.Infrastructure.Authorization;

namespace OrganisationRegistry.Acl.Internals;

public interface IAclRunnable { Result Run(); }
public interface IRulesProvider<TData> { Rule<TData>[] For(Role role, Capability capability); }
public interface IOperationRequest { CrudOperation Ops { get; } }

public sealed class AclRuntimeBuilder<TArgs>(IRulesProvider<TArgs> rulesProvider)
{
    public AclRuntime<TArgs> Build(
        Role role,
        Capability resource,
        TArgs args)

    => new(rulesProvider.For(role, resource), args);
}

public static class AclRuntime
{
    public static Result Run<TArgs>(Rule<TArgs>[] rules, TArgs args)
    {
        foreach (var rule in rules)
        {
            var result = rule(args);
            if (!result.IsSuccess) return result;
        }

        return Result.Success;
    }
}

public static class AclHost
{
    public static Result Exec<TRuntime>(TRuntime runner) where TRuntime : IAclRunnable => runner.Run();
}

public readonly struct AclRuntime<TArgs>(Rule<TArgs>[] ruleSet, TArgs args) : IAclRunnable
{
    public Result Run() => AclRuntime.Run(ruleSet, args);
}

public readonly record struct Result(bool IsSuccess, string? Reason)
{
    public static readonly Result Success = new(true, null);
    public static Result Failed(string? reason = null) => new(false, reason);
}

public delegate Result Rule<TData>(TData args);
public static class Rule
{
    public static Rule<T> Define<T>(CrudOperation ops) where T : IOperationRequest
        => data => (ops & data.Ops) == data.Ops
            ? Result.Success
            : Result.Failed($"{data.Ops & ~ops} not granted");
}
