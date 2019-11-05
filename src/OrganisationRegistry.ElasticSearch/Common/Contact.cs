namespace OrganisationRegistry.ElasticSearch.Common
{
    using System;
    using Nest;

    public class Contact
    {
        public Guid ContactTypeId { get; set; }
        public string ContactTypeName { get; set; }
        public string Value { get; set; }

        public Contact(Guid contactTypeId, string contactTypeName, string value)
        {
            ContactTypeId = contactTypeId;
            ContactTypeName = contactTypeName;
            Value = value;
        }

        protected Contact() { }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<Contact> map) => map
            .Keyword(k => k
                .Name(p => p.ContactTypeId))

            .Text(t => t
                .Name(p => p.ContactTypeName)
                .Fields(x => x.Keyword(y => y.Name("keyword"))))

            .Text(t => t
                .Name(p => p.Value)
                .Fields(x => x.Keyword(y => y.Name("keyword"))));
    }
}
