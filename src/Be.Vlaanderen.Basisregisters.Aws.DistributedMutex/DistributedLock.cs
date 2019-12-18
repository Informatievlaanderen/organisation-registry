namespace Be.Vlaanderen.Basisregisters.Aws.DistributedMutex
{
    using System;
    using System.Timers;
    using Amazon;
    using Amazon.DynamoDBv2;

    public class DistributedLockOptions
    {
        public RegionEndpoint Region { get; set; }

        public string AwsAccessKeyId { get; set; }
        public string AwsSecretAccessKey { get; set; }

        public string TableName { get; set; } = "__DistributedLocks__";

        public TimeSpan LeasePeriod { get; set; } = TimeSpan.FromMinutes(5);

        public bool ThrowOnFailedRenew { get; set; } = true;
        public bool TerminateApplicationOnFailedRenew { get; set; } = true;
    }

    public class DistributedLock<T>
    {
        private readonly DistributedLockOptions _options;
        private readonly string _lockName;

        private readonly DynamoDBMutex _mutex;
        private readonly Timer _renewLeaseTimer = new Timer();

        private LockToken _lockToken;

        public DistributedLock(DistributedLockOptions options)
        {
            _options = options;

            _lockName = typeof(T).FullName ?? Guid.NewGuid().ToString("N");

            _mutex = new DynamoDBMutex(
                new AmazonDynamoDBClient(
                    options.AwsAccessKeyId,
                    options.AwsSecretAccessKey,
                    options.Region),
                new DynamoDBMutexSettings
                {
                    CreateTableIfNotExists = true,
                    TableName = options.TableName
                });

            _renewLeaseTimer.Interval = options.LeasePeriod.TotalMilliseconds / 2;
            _renewLeaseTimer.Elapsed += (sender, args) => RenewLease();
        }

        public bool AcquireLock()
        {
            _lockToken = _mutex.AcquireLockAsync(_lockName, _options.LeasePeriod).GetAwaiter().GetResult();

            _renewLeaseTimer.Start();

            return _lockToken != null;
        }

        public void ReleaseLock()
        {
            _mutex.ReleaseLockAsync(_lockToken).GetAwaiter().GetResult();
            _lockToken = null;
            _renewLeaseTimer.Stop();
        }

        private void RenewLease()
        {
            if (_lockToken == null)
                return;

            _lockToken = _mutex.RenewAsync(_lockToken, _options.LeasePeriod).GetAwaiter().GetResult();

            if (_lockToken == null && _options.ThrowOnFailedRenew)
                throw new Exception("Failed to renew lease.");

            if (_lockToken == null && _options.TerminateApplicationOnFailedRenew)
                Environment.Exit(1);
        }
    }
}
