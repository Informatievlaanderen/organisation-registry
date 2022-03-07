namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationFormalFramework
        {
            public Guid OrganisationFormalFrameworkId { get; set; }
            public Guid FormalFrameworkId { get; set; }
            public string FormalFrameworkName { get; set; }
            public Guid ParentOrganisationId { get; set; }
            public string ParentOrganisationName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationFormalFramework()
            {
                FormalFrameworkName = string.Empty;
                ParentOrganisationName = string.Empty;
                Validity = new Period(); // default infinitely valid
            }

            public OrganisationFormalFramework(
                Guid organisationFormalFrameworkId,
                Guid formalFrameworkId,
                string formalFrameworkName,
                Guid parentOrganisationId,
                string parentOrganisationName,
                Period validity)
            {
                OrganisationFormalFrameworkId = organisationFormalFrameworkId;

                FormalFrameworkId = formalFrameworkId;
                FormalFrameworkName = formalFrameworkName;

                ParentOrganisationId = parentOrganisationId;
                ParentOrganisationName = parentOrganisationName;

                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationFormalFramework> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationFormalFrameworkId))
                    .Keyword(
                        k => k
                            .Name(p => p.FormalFrameworkId))
                    .Text(
                        t => t
                            .Name(p => p.FormalFrameworkName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
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
}
