namespace OrganisationRegistry.UnitTests
{
    using System;

    public class DateTimeProviderStub
        : IDateTimeProvider
    {
        public DateTime Now
            => StubDate;

        public DateTime Today
            => StubDate.Date;

        public DateTimeOffset UtcNow
            => StubDate;

        public DateTime StubDate { get; set; }

        public DateTimeProviderStub(DateTime date)
            => StubDate = date;
    }
}
