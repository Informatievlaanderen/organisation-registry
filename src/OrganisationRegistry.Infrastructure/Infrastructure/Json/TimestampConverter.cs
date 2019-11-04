namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using Newtonsoft.Json.Converters;

    public class TimestampConverter : IsoDateTimeConverter
    {
        public TimestampConverter() => DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    }
}
