namespace OrganisationRegistry
{
    using System;

    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => Now.Date;
    }
}
