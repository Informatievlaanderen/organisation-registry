namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using Osc;
using Newtonsoft.Json;

public partial class OrganisationDocument
{
    public class OrganisationBuilding
    {
        public Guid OrganisationBuildingId { get; set; }
        public Guid BuildingId { get; set; }
        public string BuildingName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool IsMainBuilding { get; set; }

        public Period Validity { get; set; }

        protected OrganisationBuilding()
        {
            BuildingName = string.Empty;
            Validity = Period.Infinite();
        }

        public OrganisationBuilding(
            Guid organisationBuildingId,
            Guid buildingId,
            string buildingName,
            bool isMainBuilding,
            Period validity)
        {
            OrganisationBuildingId = organisationBuildingId;
            BuildingId = buildingId;
            BuildingName = buildingName;
            IsMainBuilding = isMainBuilding;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBuilding> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationBuildingId))
                .Keyword(
                    k => k
                        .Name(p => p.BuildingId))
                .Text(
                    t => t
                        .Name(p => p.BuildingName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Boolean(
                    t => t
                        .Name(p => p.IsMainBuilding))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}
