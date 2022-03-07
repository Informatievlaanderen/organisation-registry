namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Osc;

    public partial class OrganisationDocument
    {
        public class OrganisationFunction
        {
            public Guid OrganisationFunctionId { get; set; }
            public Guid FunctionId { get; set; }
            public string FunctionName { get; }
            public Guid PersonId { get; set; }
            public string PersonName { get; }
            public List<Contact> Contacts { get; set; }
            public Period Validity { get; set; }

            protected OrganisationFunction()
            {
            }

            public OrganisationFunction(
                Guid organisationFunctionId,
                Guid functionId,
                string functionName,
                Guid personId,
                string personName,
                List<Contact> contacts,
                Period validity)
            {
                OrganisationFunctionId = organisationFunctionId;
                FunctionId = functionId;
                FunctionName = functionName;
                PersonId = personId;
                PersonName = personName;
                Contacts = contacts;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationFunction> map)
                => map
                    .Keyword(
                        k => k
                            .Name(p => p.OrganisationFunctionId))
                    .Keyword(
                        k => k
                            .Name(p => p.FunctionId))
                    .Text(
                        t => t
                            .Name(p => p.FunctionName)
                            .Fields(x => x.Keyword(y => y.Name("keyword"))))
                    .Keyword(
                        k => k
                            .Name(p => p.PersonId))
                    .Text(
                        t => t
                            .Name(p => p.PersonName)
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
