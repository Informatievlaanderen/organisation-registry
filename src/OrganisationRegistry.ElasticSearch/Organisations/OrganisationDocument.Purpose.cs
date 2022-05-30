namespace OrganisationRegistry.ElasticSearch.Organisations;

using Osc;
using System;

public partial class OrganisationDocument
{
    public class Purpose
    {
        public Guid PurposeId { get; set; }
        public string PurposeName { get; set; }

#pragma warning disable CS8618
        protected Purpose()
#pragma warning restore CS8618
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