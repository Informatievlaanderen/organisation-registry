namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
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
        protected abstract IEnumerable<IEvent> Given();
        protected abstract TEvent When();
        protected abstract TReactionHandler BuildReactionHandler(IContextFactory contextFactory);
        protected abstract int ExpectedNumberOfCommands { get; }

        protected IList<ICommand> Commands { get; }

        protected ReactionCommandsTestBase()
        {
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

            var context = new OrganisationRegistryContext(ContextOptions);

            var memoryCaches = new MemoryCaches(context);
            var memoryCachesMaintainer = new MemoryCachesMaintainer(memoryCaches);
            var reactionHandler = BuildReactionHandler(new TestContextFactory(ContextOptions));

            HandleEvents(reactionHandler, memoryCachesMaintainer, Given().ToArray());

            var envelope = (dynamic)When().ToEnvelope();
            Commands = reactionHandler.Handle(envelope).GetAwaiter().GetResult();
        }

        public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; set; }

        private async Task HandleEvents(object reactionHandler, MemoryCachesMaintainer memoryCaches, params IEvent[] events)
        {
            foreach (var @event in events)
            {
                await Publish(memoryCaches, @event);
                await Publish(reactionHandler, @event);
            }
        }

        private async Task Publish(object eventHandler, IEvent @event)
        {
            var type = eventHandler.GetType();
            var envelope = @event.ToEnvelope();
            var taskType = Type.MakeGenericSignatureType(typeof(Task), typeof(DbConnection), typeof(DbTransaction), envelope.GetType());

            var methodInfo = type.GetMethod("Handle", new[] {typeof(DbConnection), typeof(DbTransaction), envelope.GetType()});
            var task = (Task)methodInfo?.Invoke(eventHandler, new object[] {null, null, envelope});
            if (task != null)
                await task;
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
