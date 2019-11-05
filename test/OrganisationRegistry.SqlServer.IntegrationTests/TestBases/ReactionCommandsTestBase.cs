namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure;
    using OnProjections;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using Xunit;

    /// <summary>
    /// Use ReactionCommandsTestBase if you want assert the commands created by a reaction to an event.
    /// </summary>
    /// <typeparam name="TReactionHandler"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class ReactionCommandsTestBase<TReactionHandler, TEvent> : IDisposable
        where TReactionHandler : class, IReactionHandler<TEvent>
        where TEvent : IEvent<TEvent>
    {
        public IServiceProvider FixtureServiceProvider;
        public MemoryCaches MemoryCaches;
        public string FixtureConnectionString { get; }
        public SqlConnection SqlConnection { get; }
        public DbTransaction Transaction { get; }
        protected OrganisationRegistryContext Context { get; }


        protected abstract IEnumerable<IEvent> Given();
        protected abstract TEvent When();
        protected abstract TReactionHandler BuildReactionHandler();
        protected abstract int ExpectedNumberOfCommands { get; }

        protected IList<ICommand> Commands { get; }

        protected ReactionCommandsTestBase(SqlServerFixture fixture)
        {
            FixtureConnectionString = fixture.ConnectionString;
            FixtureServiceProvider = fixture.ServiceProvider;

            SqlConnection = new SqlConnection(FixtureConnectionString);
            SqlConnection.Open();
            Transaction = SqlConnection.BeginTransaction(IsolationLevel.Serializable);
            Context = new OrganisationRegistryTransactionalContext(SqlConnection, Transaction);


            MemoryCaches = new MemoryCaches(Context);
            var memoryCachesMaintainer = new MemoryCachesMaintainer(MemoryCaches);
            var reactionHandler = BuildReactionHandler();

            HandleEvents(reactionHandler, memoryCachesMaintainer, Given().ToArray());

            Commands = reactionHandler.Handle((dynamic)When().ToEnvelope());
        }

        private void HandleEvents(object reactionHandler, MemoryCachesMaintainer memoryCaches, params IEvent[] events)
        {
            foreach (var @event in events)
            {
                Publish(memoryCaches, @event);
                Publish(reactionHandler, @event);
            }
        }

        private void Publish(object eventHandler, IEvent @event)
        {
            var type = eventHandler.GetType();
            var envelope = @event.ToEnvelope();
            var methodInfo = type.GetMethod("Handle", new[] {typeof(DbConnection), typeof(DbTransaction), envelope.GetType()});
            methodInfo?.Invoke(eventHandler, new object[] {SqlConnection, Transaction, envelope});
        }

        [Fact]
        public void PublishesTheCorrectNumberOfCommands()
        {
            Commands.Count.Should().Be(ExpectedNumberOfCommands);
        }

        public void Dispose()
        {
            Transaction.Rollback();
            Context.Dispose();
        }
    }
}
