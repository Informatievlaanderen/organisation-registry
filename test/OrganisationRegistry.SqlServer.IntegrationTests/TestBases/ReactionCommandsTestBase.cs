namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
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
        private OrganisationRegistryContext _context;
        public string FixtureConnectionString { get; }
        public SqlConnection SqlConnection { get; }
        public DbTransaction Transaction { get; }
        protected OrganisationRegistryContext Context { get; }


        protected abstract IEnumerable<IEvent> Given();
        protected abstract TEvent When();
        protected abstract TReactionHandler BuildReactionHandler(Func<OrganisationRegistryContext> context);
        protected abstract int ExpectedNumberOfCommands { get; }

        protected IList<ICommand> Commands { get; }

        protected ReactionCommandsTestBase(SqlServerFixture fixture)
        {
            // FixtureConnectionString = fixture.ConnectionString;
            // FixtureServiceProvider = fixture.ServiceProvider;
            //
            // SqlConnection = new SqlConnection(FixtureConnectionString);
            // SqlConnection.Open();
            // Transaction = SqlConnection.BeginTransaction(IsolationLevel.Serializable);
            // Context = new OrganisationRegistryTransactionalContext(SqlConnection, Transaction);

            Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    $"org-es-test-{Guid.NewGuid()}",
                    builder => { }).Options;

            _context = new OrganisationRegistryContext(ContextOptions);

            MemoryCaches = new MemoryCaches(_context);
            var memoryCachesMaintainer = new MemoryCachesMaintainer(MemoryCaches);
            var reactionHandler = BuildReactionHandler(() => new OrganisationRegistryContext(ContextOptions));

            HandleEvents(reactionHandler, memoryCachesMaintainer, Given().ToArray());

            Commands = reactionHandler.Handle((dynamic)When().ToEnvelope());
        }

        public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; set; }

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
        }
    }
}
