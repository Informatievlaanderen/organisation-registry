namespace OrganisationRegistry.Handling;

using System;
using System.Threading.Tasks;
using Authorization;
using Infrastructure.Authorization;
using Infrastructure.Domain;

/// <summary>
///     Basic handler that can handle all sorts of scenarios
/// </summary>
public class Handler
{
    private readonly ISession _session;
    private readonly IUser _user;
    private ISecurityPolicy? _policy;

    private Handler(IUser user, ISession session)
    {
        _user = user;
        _session = session;
    }

    public static Handler For(IUser user, ISession session)
        => new(user, session);

    public Handler WithPolicy(ISecurityPolicy policy)
    {
        _policy = policy;
        return this;
    }

    public async Task Handle(Func<ISession, Task> handle)
    {
        var result = _policy?.Check(_user);

        if (result?.Exception is { } exception)
            throw exception;

        await handle(_session);
        await _session.Commit(_user);
    }

    public async Task Handle(Action<ISession> handle)
    {
        var result = _policy?.Check(_user);

        if (result?.Exception is { } exception)
            throw exception;

        handle(_session);
        await _session.Commit(_user);
    }

    public async Task HandleWithCombinedTransaction(Action<ISession> handle)
    {
        var result = _policy?.Check(_user);

        if (result?.Exception is { } exception)
            throw exception;

        handle(_session);
        await _session.CommitAllInOneTransaction(_user);
    }
}
