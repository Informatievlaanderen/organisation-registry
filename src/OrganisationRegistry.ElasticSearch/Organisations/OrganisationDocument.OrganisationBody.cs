namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationBody
        {
            public Guid BodyOrganisationId { get; set; }
            public Guid BodyId { get; set; }
            public string BodyName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationBody()
            {
                BodyName = string.Empty;
                Validity = Period.Infinite();
            }

            public OrganisationBody(
                Guid bodyOrganisationId,
                Guid bodyId,
                string bodyName,
                Period validity)
            {
                BodyOrganisationId = bodyOrganisationId;
                BodyId = bodyId;
                BodyName = bodyName;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBody> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.BodyOrganisationId))
                    .Keyword(
                        k => k
                            .Name(p => p.BodyId))
                    .Text(
                        t => t
                            .Name(p => p.BodyName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Object<Period>(
                        o => o
                            .Name(p => p.Validity)
                            .Properties(Period.Mapping));
        }
    }
}
