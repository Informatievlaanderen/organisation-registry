namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationCapacity
        {
            public Guid OrganisationCapacityId { get; set; }
            public Guid CapacityId { get; set; }
            public string CapacityName { get; set; }
            public Guid? PersonId { get; set; }
            public string PersonName { get; set; }
            public Guid? FunctionId { get; set; }
            public string FunctionName { get; set; }
            public List<Contact> Contacts { get; set; }
            public Period Validity { get; set; }

            protected OrganisationCapacity()
            {
                CapacityName = string.Empty;
                PersonName = string.Empty;
                FunctionName = string.Empty;
                Contacts = new List<Contact>();
                Validity = Period.Infinite();
            }

            public OrganisationCapacity(
                Guid organisationCapacityId,
                Guid capacityId,
                string capacityName,
                Guid? personId,
                string personName,
                Guid? functionId,
                string functionName,
                List<Contact> contacts,
                Period validity)
            {
                OrganisationCapacityId = organisationCapacityId;
                CapacityId = capacityId;
                PersonId = personId;
                FunctionId = functionId;
                Contacts = contacts;
                Validity = validity;
                CapacityName = capacityName;
                PersonName = personName;
                FunctionName = functionName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationCapacity> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationCapacityId))
                    .Keyword(
                        k => k
                            .Name(p => p.CapacityId))
                    .Text(
                        t => t
                            .Name(p => p.CapacityName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Keyword(
                        k => k
                            .Name(p => p.PersonId))
                    .Text(
                        t => t
                            .Name(p => p.PersonName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Keyword(
                        k => k
                            .Name(p => p.FunctionId))
                    .Text(
                        t => t
                            .Name(p => p.FunctionName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Nested<Contact>(
                        n => n
                            .Name(p => p.Contacts)
                            .IncludeInRoot()
                            .Properties(Contact.Mapping))
                    .Object<Period>(
                        o => o
                            .Name(p => p.Validity)
                            .Properties(Period.Mapping));
        }
    }
}
