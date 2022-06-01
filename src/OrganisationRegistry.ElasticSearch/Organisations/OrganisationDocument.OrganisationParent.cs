namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using Osc;

public partial class OrganisationDocument
{
    public class OrganisationParent
    {
        public Guid OrganisationOrganisationParentId { get; set; }
        public Guid ParentOrganisationId { get; set; }
        public string ParentOrganisationName { get; set; }
        public Period Validity { get; set; }

        protected OrganisationParent()
        {
            Validity = Period.Infinite();
            ParentOrganisationName = string.Empty;
        }

        public OrganisationParent(
            Guid organisationOrganisationParentId,
            Guid parentOrganisationId,
            string parentOrganisationName,
            Period validity)
        {
            OrganisationOrganisationParentId = organisationOrganisationParentId;
            ParentOrganisationId = parentOrganisationId;
            Validity = validity;
            ParentOrganisationName = parentOrganisationName;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationParent> map)
            => map
                .Keyword(
                    k => k
                        .Name(p => p.OrganisationOrganisationParentId))
                .Keyword(
                    k => k
                        .Name(p => p.ParentOrganisationId))
                .Text(
                    t => t
                        .Name(p => p.ParentOrganisationName)
                        .Fields(x => x.Keyword(y => y.Name("keyword"))))
                .Object<Period>(
                    o => o
                        .Name(p => p.Validity)
                        .Properties(Period.Mapping));
    }
}
