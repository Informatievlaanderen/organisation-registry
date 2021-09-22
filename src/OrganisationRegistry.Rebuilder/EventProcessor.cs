namespace OrganisationRegistry.Rebuilder
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Infrastructure.Events;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SqlServer;

    public class EventProcessor : IHostedService
    {
        private readonly Channel<object> _messageChannel;
        private readonly CancellationTokenSource _messagePumpCancellation;
        private readonly Task _messagePump;

        private readonly Scheduler _scheduler;
        private readonly ILogger<EventProcessor> _logger;

        private static readonly TimeSpan ResumeAfter = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan WhenNotEnabledResumeAfter = TimeSpan.FromMinutes(2);

        private const int CatchUpBatchSize = 2000;

        public EventProcessor(Scheduler scheduler,
            ILogger<EventProcessor> logger,
            IEventPublisher publisher,
            IEventStore store,
            IOptions<InfrastructureConfiguration> infrastructureOption,
            IContextFactory contextFactory)
        {
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _messagePumpCancellation = new CancellationTokenSource();
            _messageChannel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });

            var number = -1;

            _messagePump = Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        logger.LogInformation("[{Context}] EventProcessor message pump entered ...", "body");
                        while (await _messageChannel.Reader.WaitToReadAsync(_messagePumpCancellation.Token)
                            .ConfigureAwait(false))
                        {
                            while (_messageChannel.Reader.TryRead(out var message))
                            {
                                switch (message)
                                {
                                    case Resume _:
                                        logger.LogInformation("Resuming ...");

                                        using (var context = contextFactory.Create())
                                        {
                                            var toggle = await context.Configuration.SingleOrDefaultAsync(item =>
                                                item.Key == $"{TogglesConfiguration.Section}:{nameof(TogglesConfiguration.ElasticSearchProjectionsAvailable)}");
                                            if (!bool.TryParse(toggle?.Value, out var enabled) || enabled)
                                            {
                                                await _messageChannel.Writer
                                                    .WriteAsync(new CatchUp(CatchUpBatchSize),
                                                        _messagePumpCancellation.Token)
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                await scheduler.Schedule(async token =>
                                                {
                                                    if (!_messagePumpCancellation.IsCancellationRequested)
                                                    {
                                                        await _messageChannel.Writer
                                                            .WriteAsync(
                                                                new Resume(), _messagePumpCancellation.Token)
                                                            .ConfigureAwait(false);
                                                    }
                                                }, WhenNotEnabledResumeAfter).ConfigureAwait(false);
                                            }
                                        }

                                        break;
                                    case CatchUp catchUp:
                                        try
                                        {
                                            Console.WriteLine($"Catching up from {number}");
                                            var events = store.GetEventEnvelopesAfter(number, 5000);

                                            await using var connection = new SqlConnection(infrastructureOption.Value.EventStoreConnectionString);
                                            await connection.OpenAsync();
                                            await using (var tx = await connection.BeginTransactionAsync(IsolationLevel.Serializable))
                                            {
                                                foreach (var @event in events)
                                                {
                                                    await publisher.Publish(connection, tx, (dynamic)@event);
                                                    number = @event.Number;
                                                }

                                                await tx.CommitAsync();
                                            }

                                            await scheduler.Schedule(async token =>
                                            {
                                                if (!_messagePumpCancellation.IsCancellationRequested)
                                                {
                                                    await _messageChannel.Writer
                                                        .WriteAsync(
                                                            new Resume(), _messagePumpCancellation.Token)
                                                        .ConfigureAwait(false);
                                                }
                                            }, ResumeAfter).ConfigureAwait(false);

                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogCritical(0, ex,
                                                "An exception occurred while handling envelopes.");

                                            await scheduler.Schedule(async token =>
                                            {
                                                if (!_messagePumpCancellation.IsCancellationRequested)
                                                {
                                                    await _messageChannel.Writer
                                                        .WriteAsync(new Resume(), _messagePumpCancellation.Token)
                                                        .ConfigureAwait(false);
                                                }
                                            }, ResumeAfter).ConfigureAwait(false);
                                        }

                                        break;
                                }
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogInformation("EventProcessor message pump is exiting due to cancellation");
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogInformation("EventProcessor message pump is exiting due to cancellation");
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "EventProcessor message pump is exiting due to a bug");
                    }
                }, _messagePumpCancellation.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }


        private class Resume
        {
        }

        private class CatchUp
        {
            public CatchUp(int batchSize)
            {
                BatchSize = batchSize;
            }

            public int BatchSize { get; }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{Context}] Starting event processor ...", "body");
            await _scheduler.StartAsync(cancellationToken).ConfigureAwait(false);
            await _messageChannel.Writer.WriteAsync(new Resume(), cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("[{Context}] Started event processor.", "body");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{Context}] Stopping event processor ...", "body");
            _messageChannel.Writer.Complete();
            _messagePumpCancellation.Cancel();
            await _messagePump.ConfigureAwait(false);
            _messagePumpCancellation.Dispose();
            await _scheduler.StopAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("[{Context}] Stopped event processor.", "body");
        }
    }
}
