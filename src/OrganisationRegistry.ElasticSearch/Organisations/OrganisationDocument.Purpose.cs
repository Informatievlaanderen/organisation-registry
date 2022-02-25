namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using Nest;
    using System;

    public partial class OrganisationDocument
    {
        public class Purpose
        {
            public Guid PurposeId { get; set; }
            public string PurposeName { get; set; }

            protected Purpose()
            {
            }

            public Purpose(Guid purposeId, string purposeName)
            {
                PurposeId = purposeId;
                PurposeName = purposeName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<Purpose> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.PurposeId))
                    .Text(
                        t => t
                            .Name(p => p.PurposeName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))));
        }
    }
}
