namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationOrganisationClassification
        {
            public Guid OrganisationOrganisationClassificationId { get; set; }
            public Guid OrganisationClassificationTypeId { get; set; }
            public string OrganisationClassificationTypeName { get; set; }
            public Guid OrganisationClassificationId { get; set; }
            public string OrganisationClassificationName { get; set; }
            public Period Validity { get; set; }

#pragma warning disable CS8618
            protected OrganisationOrganisationClassification()
#pragma warning restore CS8618
            {
            }

            public OrganisationOrganisationClassification(
                Guid organisationOrganisationClassificationId,
                Guid organisationClassificationTypeId,
                string organisationClassificationTypeName,
                Guid organisationClassificationId,
                string organisationClassificationName,
                Period validity)
            {
                OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
                OrganisationClassificationTypeId = organisationClassificationTypeId;
                OrganisationClassificationId = organisationClassificationId;
                Validity = validity;
                OrganisationClassificationTypeName = organisationClassificationTypeName;
                OrganisationClassificationName = organisationClassificationName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationOrganisationClassification> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationOrganisationClassificationId))
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationClassificationTypeId))
                    .Text(
                        t => t
                            .Name(p => p.OrganisationClassificationTypeName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationClassificationId))
                    .Text(
                        t => t
                            .Name(p => p.OrganisationClassificationName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Object<Period>(
                        o => o
                            .Name(p => p.Validity)
                            .Properties(Period.Mapping));
        }
    }
}
