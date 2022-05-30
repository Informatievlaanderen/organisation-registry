namespace OrganisationRegistry.Handling;

using System;
using System.Threading.Tasks;
using Authorization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Infrastructure.Authorization;
using Infrastructure.Domain;

/// <summary>
///     Handler specialized in handling scenarios where you want to update an aggregate root
/// </summary>
/// <typeparam name="T">The type of aggregate root</typeparam>
public class UpdateHandler<T> where T : AggregateRoot
{
    private readonly T _aggregateRoot;
    private readonly IUser _user;
    private readonly ISession _session;
    private ISecurityPolicy? _policy;

    private UpdateHandler(ISession session, T aggregateRoot, IUser user)
    {
        _session = session;
        _aggregateRoot = aggregateRoot;
        _user = user;
    }

    public static UpdateHandler<T> For<TCommand>(BaseCommand<TCommand> command, IUser user, ISession session)
        where TCommand : GuidValueObject<TCommand>
    {
        var commandId = command.Id;
        var aggregate = session.Get<T>(commandId);
        return new UpdateHandler<T>(session, aggregate, user);
    }

    public UpdateHandler<T> WithPolicy(Func<T, ISecurityPolicy> policy)
    {
        _policy = policy(_aggregateRoot);
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
}