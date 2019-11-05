namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore
{
    using System;

    public class DateTimeProviderStub
        : IDateTimeProvider
    {
        public DateTime Now => StubDate;

        public DateTime Today => StubDate.Date;

        public DateTime StubDate { get; set; }

        public DateTimeProviderStub(DateTime date)
        {
            StubDate = date;
        }
    }
}
