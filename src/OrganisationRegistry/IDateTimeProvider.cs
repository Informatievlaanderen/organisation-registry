namespace OrganisationRegistry
{
    using System;

    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTimeOffset UtcNow { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTime Today => Now.Date;
    }
}
