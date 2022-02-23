namespace OrganisationRegistry.Handling
{
    using System;
    using System.Threading.Tasks;
    using Authorization;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Infrastructure.Domain;

    /// <summary>
    /// Handler specialized in handling scenarios where you want to update an aggregate root
    /// </summary>
    /// <typeparam name="T">The type of aggregate root</typeparam>
    public class UpdateHandler<T> where T : AggregateRoot
    {
        private readonly BaseCommand _command;
        private readonly ISession _session;
        private readonly T _aggregateRoot;
        private ISecurityPolicy? _policy;

        private UpdateHandler(BaseCommand command, ISession session, T aggregateRoot)
        {
            _command = command;
            _session = session;
            _aggregateRoot = aggregateRoot;
        }

        public static UpdateHandler<T> For<TCommand>(BaseCommand<TCommand> command, ISession session) where TCommand : GuidValueObject<TCommand>
        {
            var commandId = command.Id;
            var aggregate = session.Get<T>(commandId);
            return new UpdateHandler<T>(command, session, aggregate);
        }

        public UpdateHandler<T> WithPolicy(Func<T, ISecurityPolicy> policy)
        {
            _policy = policy(_aggregateRoot);
            return this;
        }

        public async Task Handle(Func<ISession, Task> handle)
        {
            var user = _command.User;

            var result = _policy?.Check(user);

            if (result?.Exception != null)
                throw result.Exception!;

            await handle(_session);
            await _session.Commit(user);
        }

        public async Task Handle(Action<ISession> handle)
        {
            var user = _command.User;

            var result = _policy?.Check(user);

            if (result?.Exception != null)
                throw result.Exception!;

            handle(_session);
            await _session.Commit(user);
        }
    }
}
