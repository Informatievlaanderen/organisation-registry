namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Body;
    using Cache;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Organisations;
    using People;
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
            BodyRunner bodyRunner,
            PeopleRunner peopleRunner,
            CacheRunner cacheRunner,
            IndividualRebuildRunner individualRebuildRunner,
            OrganisationsRunner organisationsRunner,
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

            _messagePump = Task.Factory.StartNew(
                async () =>
                {
                    var elasticRunners = new ElasticRunners(bodyRunner, peopleRunner, cacheRunner, individualRebuildRunner, organisationsRunner);
                    await PumpMessages(scheduler, logger, elasticRunners, contextFactory);
                },
                _messagePumpCancellation.Token,
                TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
        }

        private async Task PumpMessages(Scheduler scheduler, ILogger<EventProcessor> logger, ElasticRunners runners, IContextFactory contextFactory)
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
                            case Resume:
                                await ResumeAction(scheduler, logger, contextFactory);
                                break;
                            case CatchUp:
                                await CatchUpAction(scheduler, runners);
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
        }

        private async Task CatchUpAction(Scheduler scheduler, ElasticRunners runners)
        {
            try
            {
                await runners.CacheRunner.Run();
                await runners.IndividualRebuildRunner.Run();
                await runners.PeopleRunner.Run();
                await runners.BodyRunner.Run();
                await runners.OrganisationsRunner.Run();

                await ScheduleResume(scheduler, ResumeAfter);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "An exception occurred while handling envelopes");

                await ScheduleResume(scheduler, ResumeAfter);
            }
        }

        private async Task ResumeAction(Scheduler scheduler, ILogger<EventProcessor> logger, IContextFactory contextFactory)
        {
            logger.LogInformation("[{Context}] Resuming ...", "body");

            using (var context = contextFactory.Create())
            {
                var toggle = await context.Configuration.SingleOrDefaultAsync(
                    item =>
                        item.Key == $"{TogglesConfigurationSection.Name}:{nameof(TogglesConfigurationSection.ElasticSearchProjectionsAvailable)}");
                if (!bool.TryParse(toggle?.Value, out var enabled) || enabled)
                {
                    await _messageChannel.Writer
                        .WriteAsync(new CatchUp(CatchUpBatchSize), _messagePumpCancellation.Token)
                        .ConfigureAwait(false);
                }
                else
                {
                    await ScheduleResume(scheduler, WhenNotEnabledResumeAfter);
                }
            }
        }

        private async Task ScheduleResume(Scheduler scheduler, TimeSpan resumeAfter)
            => await scheduler.Schedule(
                async _ =>
                {
                    if (!_messagePumpCancellation.IsCancellationRequested)
                    {
                        await _messageChannel.Writer
                            .WriteAsync(new Resume(), _messagePumpCancellation.Token)
                            .ConfigureAwait(false);
                    }
                },
                resumeAfter).ConfigureAwait(false);

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

        private record ElasticRunners(BodyRunner BodyRunner, PeopleRunner PeopleRunner, CacheRunner CacheRunner, IndividualRebuildRunner IndividualRebuildRunner, OrganisationsRunner OrganisationsRunner);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{Context}] Starting event processor ...", "body");
            await _scheduler.StartAsync(cancellationToken).ConfigureAwait(false);
            await _messageChannel.Writer.WriteAsync(new Resume(), cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("[{Context}] Started event processor", "body");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("[{Context}] Stopping event processor ...", "body");
            _messageChannel.Writer.Complete();
            _messagePumpCancellation.Cancel();
            await _messagePump.ConfigureAwait(false);
            _messagePumpCancellation.Dispose();
            await _scheduler.StopAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("[{Context}] Stopped event processor", "body");
        }
    }
}
