namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using App.Metrics;
    using App.Metrics.Timer;
    using Bodies;
    using Body;
    using Cache;
    using Client;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Nest;
    using OrganisationRegistry.Infrastructure.Events;
    using Organisations;
    using People;
    using SqlServer.ProjectionState;
    using SqlStreamStore.Streams;
    using Position = OrganisationRegistry.Infrastructure.EventStore.Position;

    public class EventProcessor : IHostedService
    {
        private readonly Channel<object> _messageChannel;
        private readonly CancellationTokenSource _messagePumpCancellation;
        private readonly Task _messagePump;

        private readonly IEventStore _store;
        private readonly Scheduler _scheduler;
        private readonly ILogger<EventProcessor> _logger;
        private readonly IProjectionStates _projectionStates;
        private readonly Elastic _elastic;
        private readonly ElasticBus _bus;
        private readonly IMetricsRoot _metrics;

        private static readonly TimeSpan CatchUpAfter = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan ResumeAfter = TimeSpan.FromSeconds(2);

        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchBodiesProjection";
        private static readonly string ProjectionFullName = typeof(BodyHandler).FullName;
        private new const string ProjectionName = nameof(BodyHandler);


        private const int CatchUpBatchSize = 2000;

        public static readonly Type[] EventHandlers =
        {
            typeof(BodyHandler)
        };

        public EventProcessor(
            IEventStore store,
            Scheduler scheduler,
            ILogger<EventProcessor> logger,
            IProjectionStates projectionStates,
            Elastic elastic,
            ElasticBus bus,
            IMetricsRoot metrics,
            BodyRunner bodyRunner,
            PeopleRunner peopleRunner,
            CacheRunner cacheRunner,
            IndividualRebuildRunner individualRebuildRunner,
            OrganisationsRunner organisationsRunner)
        {
            _store = store;
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _projectionStates = projectionStates;
            _elastic = elastic;
            _bus = bus;
            _metrics = metrics;

            var changeDocumentTimer = new TimerOptions
            {
                Name = "Change Document Timer",
                MeasurementUnit = Unit.Calls,
                DurationUnit = App.Metrics.TimeUnit.Milliseconds,
                RateUnit = App.Metrics.TimeUnit.Milliseconds
            };

            var getEventsTimer = new TimerOptions
            {
                Name = "Get Events Timer",
                MeasurementUnit = Unit.Calls,
                DurationUnit = App.Metrics.TimeUnit.Milliseconds,
                RateUnit = App.Metrics.TimeUnit.Milliseconds
            };

            _messagePumpCancellation = new CancellationTokenSource();
            _messageChannel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
                AllowSynchronousContinuations = false
            });

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
                                        logger.LogInformation("[{Context}] Resuming ...", "body");
                                        var projectionPosition =
                                            projectionStates.GetLastProcessedEventNumber(
                                                ElasticSearchProjectionsProjectionName);

                                        await _messageChannel.Writer
                                            .WriteAsync(new CatchUp(projectionPosition, CatchUpBatchSize),
                                                _messagePumpCancellation.Token)
                                            .ConfigureAwait(false);
                                        break;
                                    case CatchUp catchUp:
                                        try
                                        {
                                            await cacheRunner.Run();
                                            await individualRebuildRunner.Run();
                                            await peopleRunner.Run();
                                            await bodyRunner.Run();
                                            await organisationsRunner.Run();
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
                                                "[{ProjectionName}] An exception occurred while handling envelopes.",
                                                ProjectionName);

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
                                        finally
                                        {
                                            await Task.WhenAll(_metrics.ReportRunner.RunAllAsync());
                                        }

                                        break;
                                }
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogInformation("[{Context}] EventProcessor message pump is exiting due to cancellation",
                            "body");
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogInformation("[{Context}] EventProcessor message pump is exiting due to cancellation",
                            "body");
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "[{Context}] EventProcessor message pump is exiting due to a bug",
                            "body");
                    }
                }, _messagePumpCancellation.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        private async Task<ElasticChanges> ProcessEnvelope(IEnvelope envelope)
        {
            try
            {
                var changes = await _bus.Publish(null, null, (dynamic) envelope);
                return new ElasticChanges(envelope.Number, changes);
            }
            catch
            {
                _logger.LogCritical(
                    "[{ProjectionName}] An exception occurred while processing envelope #{EnvelopeNumber}:{@EnvelopeJson}",
                    ProjectionName,
                    envelope.Number,
                    envelope);

                throw;
            }
        }

        private async Task InitialiseProjection(int lastProcessedEventNumber)
        {
            if (lastProcessedEventNumber != -1)
                return;

            _logger.LogInformation("[{ProjectionName}] First run, initialising projections!", ProjectionName);
            await ProcessEnvelope(new InitialiseProjection(ProjectionFullName).ToTypedEnvelope());
        }


        private void UpdateProjectionState(int? newLastProcessedEventNumber)
        {
            if (!newLastProcessedEventNumber.HasValue)
                return;

            _logger.LogInformation(
                "[{ProjectionName}] Processed up until envelope #{LastProcessedEnvelopeNumber}, writing number to db...",
                ProjectionName, newLastProcessedEventNumber);
            _projectionStates.UpdateProjectionState(ElasticSearchProjectionsProjectionName,
                newLastProcessedEventNumber.Value);
        }

        private class Resume
        {
        }

        private class CatchUp
        {
            public CatchUp(int? afterPosition, int batchSize)
            {
                AfterPosition = afterPosition;
                BatchSize = batchSize;
            }

            public int? AfterPosition { get; }
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
