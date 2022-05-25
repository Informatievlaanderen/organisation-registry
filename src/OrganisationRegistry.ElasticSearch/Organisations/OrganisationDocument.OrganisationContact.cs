namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationContact
        {
            public Guid OrganisationContactId { get; set; }
            public Guid ContactTypeId { get; set; }
            public string ContactTypeName { get; set; }
            public string Value { get; set; }
            public Period Validity { get; set; }

            protected OrganisationContact()
            {
                ContactTypeName = string.Empty;
                Value = string.Empty;
                Validity = Period.Infinite();
            }

            public OrganisationContact(
                Guid organisationContactId,
                Guid contactTypeId,
                string contactTypeName,
                string value,
                Period validity)
            {
                OrganisationContactId = organisationContactId;
                ContactTypeId = contactTypeId;
                ContactTypeName = contactTypeName;
                Value = value;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationContact> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationContactId))

                .Keyword(k => k
                    .Name(p => p.ContactTypeId))

                .Text(t => t
                    .Name(p => p.ContactTypeName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Text(t => t
                    .Name(p => p.Value)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }
    }
}
