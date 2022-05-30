namespace OrganisationRegistry.ElasticSearch.Projections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NodaTime;
using IClock = NodaTime.IClock;

public class Scheduler
{
    private static readonly TimeSpan DefaultFrequency = TimeSpan.FromSeconds(1);

    private readonly IClock _clock;
    private readonly ILogger<Scheduler> _logger;

    private readonly Timer _timer;

    private readonly CancellationTokenSource _messagePumpCancellation;
    private readonly Channel<object> _messageChannel;
    private readonly Task _messagePump;

    public Scheduler(IClock clock, ILogger<Scheduler> logger)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _messagePumpCancellation = new CancellationTokenSource();
        _messageChannel = Channel.CreateUnbounded<object>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
        _timer = new Timer(
            _ => _messageChannel.Writer.WriteAsync(new TimerElapsed { Time = _clock.GetCurrentInstant() }, _messagePumpCancellation.Token),
            null,
            Timeout.InfiniteTimeSpan,
            Timeout.InfiniteTimeSpan);
        _messagePump = CreateMessagePump();
    }

    private Task<Task> CreateMessagePump()
    {
        return Task.Factory.StartNew(async () =>
        {
            var scheduled = new List<ScheduledAction>();
            try
            {
                _logger.LogDebug("Scheduler message pump entered ...");
                while (await _messageChannel.Reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    while (_messageChannel.Reader.TryRead(out var message))
                    {
                        switch (message)
                        {
                            case TimerElapsed elapsed:
                                var dueEntries = scheduled
                                    .Where(entry => entry.Due <= elapsed.Time)
                                    .ToArray();
                                _logger.LogDebug("Timer elapsed at instant {Time}: {Length} actions due", elapsed.Time, dueEntries.Length);

                                foreach (var dueEntry in dueEntries)
                                {
                                    await dueEntry.Action(_messagePumpCancellation.Token).ConfigureAwait(false);
                                    scheduled.Remove(dueEntry);
                                }

                                if (scheduled.Count == 0) // deactivate timer when no more work
                                {
                                    _logger.LogDebug("Timer deactivated because no more scheduled actions");

                                    _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                                }

                                break;
                            case ScheduleAction schedule:
                                if (scheduled.Count == 0) // activate timer when more work
                                {
                                    _logger.LogDebug("Timer activated because new scheduled actions");

                                    _timer.Change(DefaultFrequency, DefaultFrequency);
                                }

                                _logger.LogDebug("Scheduling an action to be executed at {Due}", schedule.Due);

                                scheduled.Add(new ScheduledAction(schedule.Action, schedule.Due));
                                break;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("Scheduler message pump is exiting due to cancellation");
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug("Scheduler message pump is exiting due to cancellation");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Scheduler message pump is exiting due to a bug");
            }
        }, _messagePumpCancellation.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
    }

    public async Task Schedule(Func<CancellationToken, Task> action, TimeSpan due)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        await _messageChannel.Writer.WriteAsync(
            new ScheduleAction
            {
                Action = action,
                Due = _clock.GetCurrentInstant().Plus(Duration.FromTimeSpan(due))
            });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting scheduler ...");
        _logger.LogDebug("Started scheduler ...");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Stopping scheduler ...");
        _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _messageChannel.Writer.Complete();
        _messagePumpCancellation.Cancel();
        await _messagePump.ConfigureAwait(false);
        _messagePumpCancellation.Dispose();
        await _timer.DisposeAsync().ConfigureAwait(false);
        _logger.LogDebug("Stopped scheduler");
    }

    private class ScheduledAction
    {
        public Func<CancellationToken, Task> Action { get; }

        public Instant Due { get; }

        public ScheduledAction(Func<CancellationToken, Task> action, Instant due)
        {
            Action = action;
            Due = due;
        }
    }

    private class ScheduleAction
    {
        public Func<CancellationToken, Task> Action { get; set; } = null!;

        public Instant Due { get; set; }
    }

    private class TimerElapsed
    {
        public Instant Time { get; set; }
    }
}