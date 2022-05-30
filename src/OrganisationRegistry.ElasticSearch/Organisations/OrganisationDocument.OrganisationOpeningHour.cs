namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using Osc;

public partial class OrganisationDocument
{
    public class OrganisationOpeningHour
    {
        public Guid OrganisationOpeningHourId { get; set; }

        public TimeSpan Opens { get; set; }

        public TimeSpan Closes { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public Period Validity { get; set; }

#pragma warning disable CS8618
        protected OrganisationOpeningHour()
#pragma warning restore CS8618
        { }

        public OrganisationOpeningHour(
            Guid organisationOpeningHourId,
            TimeSpan opens,
            TimeSpan closes,
            DayOfWeek? dayOfWeek,
            Period validity)
        {
            OrganisationOpeningHourId = organisationOpeningHourId;
            Opens = opens;
            Closes = closes;
            DayOfWeek = dayOfWeek;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationOpeningHour> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationOpeningHourId))
                .Keyword(
                    k => k
                        .Name(p => p.Opens))
                .Keyword(
                    k => k
                        .Name(p => p.Closes))
                .Keyword(
                    k => k
                        .Name(p => p.DayOfWeek))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}