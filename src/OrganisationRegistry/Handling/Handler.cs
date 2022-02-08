namespace OrganisationRegistry.Handling
{
    using System;
    using System.Threading.Tasks;
    using Authorization;
    using Infrastructure.Authorization;
    using Infrastructure.Domain;

    public class Handler{
        private readonly IUser _user;
        private readonly ISession _session;
        private ISecurityPolicy? _policy;

        private Handler(IUser user, ISession session)
        {
            _user = user;
            _session = session;
        }

        public static Handler ForUser(IUser user, ISession session)
        {
            return new Handler(user, session);
        }

        public Handler WithPolicy(ISecurityPolicy policy)
        {
            _policy = policy;
            return this;
        }

        public async Task Handle(Func<ISession, Task> handle)
        {
            var result = _policy?.Check(_user, _session);

            if (result?.Exception != null)
                throw result.Exception!;

            await handle(_session);
            await _session.Commit(_user);
        }

        public async Task Handle(Action<ISession> handle)
        {
            var result = _policy?.Check(_user, _session);

            if (result?.Exception != null)
                throw result.Exception!;

            handle(_session);
            await _session.Commit(_user);
        }
    }
}
