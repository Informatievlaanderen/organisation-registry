namespace OrganisationRegistry.ElasticSearch.Organisations;

using System;
using Common;
using Osc;

public partial class OrganisationDocument
{
    public class OrganisationKey
    {
        public Guid OrganisationKeyId { get; set; }
        public Guid KeyTypeId { get; set; }
        public string KeyTypeName { get; set; }
        public string Value { get; set; }
        public Period Validity { get; set; }

        protected OrganisationKey()
        {
            KeyTypeName = string.Empty;
            Value = string.Empty;
            Validity = Period.Infinite();
        }

        public OrganisationKey(
            Guid organisationKeyId,
            Guid keyTypeId,
            string keyTypeName,
            string value,
            Period validity)
        {
            OrganisationKeyId = organisationKeyId;
            KeyTypeId = keyTypeId;
            Value = value;
            Validity = validity;
            KeyTypeName = keyTypeName;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationKey> map) => map
            .Keyword(k => k
                .Name(p => p.OrganisationKeyId))

            .Keyword(k => k
                .Name(p => p.KeyTypeId))

            .Text(t => t
                .Name(p => p.KeyTypeName)
                .Fields(x => x.Keyword(y => y.Name("keyword"))))

            .Text(t => t
                .Name(p => p.Value)
                .Fields(x => x.Keyword(y => y.Name("keyword"))))

            .Object<Period>(o => o
                .Name(p => p.Validity)
                .Properties(Period.Mapping));
    }

}