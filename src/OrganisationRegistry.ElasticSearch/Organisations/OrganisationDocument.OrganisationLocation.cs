namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Nest;
    using Newtonsoft.Json;

    public partial class OrganisationDocument
    {
        public class OrganisationLocation
        {
            public Guid OrganisationLocationId { get; set; }
            public Guid LocationId { get; set; }
            public string FormattedAddress { get; set; }

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public bool IsMainLocation { get; set; }

            public Guid? LocationTypeId { get; set; }
            public string LocationTypeName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationLocation()
            {
            }

            public OrganisationLocation(
                Guid organisationLocationId,
                Guid locationId,
                string formattedAddress,
                bool isMainLocation,
                Guid? locationTypeId,
                string locationTypeName,
                Period validity)
            {
                OrganisationLocationId = organisationLocationId;
                LocationId = locationId;
                IsMainLocation = isMainLocation;
                LocationTypeId = locationTypeId;
                LocationTypeName = locationTypeName;
                Validity = validity;
                FormattedAddress = formattedAddress;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationLocation> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationLocationId))
                    .Keyword(
                        k => k
                            .Name(p => p.LocationId))
                    .Text(
                        t => t
                            .Name(p => p.FormattedAddress)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Boolean(
                        t => t
                            .Name(p => p.IsMainLocation))
                    .Keyword(
                        k => k
                            .Name(p => p.LocationTypeId))
                    .Text(
                        t => t
                            .Name(p => p.LocationTypeName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Object<Period>(
                        o => o
                            .Name(p => p.Validity)
                            .Properties(Period.Mapping));
        }
    }
}
