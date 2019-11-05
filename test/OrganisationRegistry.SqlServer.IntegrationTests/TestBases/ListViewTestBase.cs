namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using Infrastructure;
    using OnProjections;
    using OrganisationRegistry.Infrastructure.Events;

    public abstract class ListViewTestBase : IDisposable
    {
        private readonly SqlServerFixture _fixture;
        private readonly SqlConnection _sqlConnection;
        private readonly SqlTransaction _transaction;
        protected OrganisationRegistryContext Context { get; }

        protected ListViewTestBase(SqlServerFixture fixture)
        {
            _fixture = fixture;
            _sqlConnection = new SqlConnection(fixture.ConnectionString);
            _sqlConnection.Open();
            _transaction = _sqlConnection.BeginTransaction(IsolationLevel.Serializable);
            Context = new OrganisationRegistryTransactionalContext(_sqlConnection, _transaction);
        }

        public void HandleEvents(params IEvent[] events)
        {
            foreach (var @event in events)
            {
                _fixture.Publisher.Publish(
                    _sqlConnection,
                    _transaction,
                    (dynamic) @event.ToEnvelope());
            }
        }

        public void Dispose()
        {
            _transaction.Rollback();
            _sqlConnection.Dispose();
            Context.Dispose();
        }
    }
}
