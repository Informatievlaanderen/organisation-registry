namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using OpenSearch.Client;

public partial class OrganisationDocument
{
    public class OrganisationRelation
    {
        public Guid OrganisationRelationId { get; set; }
        public Guid RelationId { get; set; }
        public string RelationName { get; }
        public Guid RelatedOrganisationId { get; set; }
        public string RelatedOrganisationOvoNumber { get; }
        public string RelatedOrganisationName { get; }
        public Period Validity { get; set; }

#pragma warning disable CS8618
        protected OrganisationRelation()
#pragma warning restore CS8618
        {
        }

        public OrganisationRelation(
            Guid organisationRelationId,
            Guid relationId,
            string relationName,
            Guid relatedOrganisationId,
            string relatedOrganisationOvoNumber,
            string relatedOrganisationName,
            Period validity)
        {
            OrganisationRelationId = organisationRelationId;
            RelationId = relationId;
            RelationName = relationName;
            RelatedOrganisationId = relatedOrganisationId;
            RelatedOrganisationOvoNumber = relatedOrganisationOvoNumber;
            RelatedOrganisationName = relatedOrganisationName;
            Validity = validity;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationRelation> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationRelationId))
                .Keyword(
                    k => k
                        .Name(p => p.RelationId))
                .Text(
                    t => t
                        .Name(p => p.RelationName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Keyword(
                    k => k
                        .Name(p => p.RelatedOrganisationId))
                .Keyword(
                    k => k
                        .Name(p => p.RelatedOrganisationOvoNumber))
                .Text(
                    t => t
                        .Name(p => p.RelatedOrganisationName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}
