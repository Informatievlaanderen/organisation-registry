namespace OrganisationRegistry.Api.ScheduledCommands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Task? _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new();
        private static int _runCounter;

        protected BackgroundService(ILogger logger)
        {
            _logger = logger;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            // Otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
                return;

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                    cancellationToken));
            }
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _runCounter, 1, 0) != 0)
                return;

            try
            {
                await Process(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while processing scheduled commands");
            }
            finally
            {
                Interlocked.Decrement(ref _runCounter);
            }
        }

        protected abstract Task Process(CancellationToken cancellationToken);

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
            _stoppingCts.Cancel();
        }
    }
}
