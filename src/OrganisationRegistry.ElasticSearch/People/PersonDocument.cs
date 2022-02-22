namespace OrganisationRegistry.ElasticSearch.People
{
    using Nest;
    using System;
    using System.Collections.Generic;
    using Common;

    public class PersonDocument : IDocument
    {
        public int ChangeId { get; set; }
        public DateTimeOffset ChangeTime { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string FirstName { get; set; }

        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public bool? ShowOnVlaamseOverheidSites { get; set; }

        public static TypeMappingDescriptor<PersonDocument> Mapping(TypeMappingDescriptor<PersonDocument> map) => map
            .Properties(ps => ps
                .Number(n => n
                    .Name(p => p.ChangeId))

                .Date(d => d
                    .Name(p => p.ChangeTime))

                .Keyword(k => k
                    .Name(p => p.Id))

                .Text(t => t
                    .Name(p => p.FirstName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(b => b
                    .Name(p => p.ShowOnVlaamseOverheidSites))

                .Text(t => t
                    .Name(p => p.Name)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Nested<PersonFunction>(n => n
                    .Name(p => p.Functions)
                    .IncludeInRoot()
                    .Properties(PersonFunction.Mapping))

                .Nested<PersonCapacity>(n => n
                    .Name(p => p.Capacities)
                    .IncludeInRoot()
                    .Properties(PersonCapacity.Mapping))

                .Nested<PersonMandate>(n => n
                    .Name(p => p.Mandates)
                    .IncludeInRoot()
                    .Properties(PersonMandate.Mapping))
            );

        public List<PersonFunction> Functions { get; set; }
        public class PersonFunction
        {
            public Guid PersonFunctionId { get; set; }
            public Guid FunctionId { get; set; }
            public string FunctionName { get; }
            public Guid OrganisationId { get; set; }
            public string OrganisationName { get; }
            public List<Contact> Contacts { get; set; }
            public Period Validity { get; set; }

            public PersonFunction(
                Guid personFunctionId,
                Guid functionId,
                string functionName,
                Guid organisationId,
                string organisationName,
                List<Contact> contacts,
                Period validity)
            {
                PersonFunctionId = personFunctionId;
                FunctionId = functionId;
                FunctionName = functionName;
                OrganisationId = organisationId;
                OrganisationName = organisationName;
                Contacts = contacts;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<PersonFunction> map) => map
                .Keyword(k => k
                    .Name(p => p.PersonFunctionId))

                .Keyword(k => k
                    .Name(p => p.FunctionId))

                .Text(t => t
                    .Name(p => p.FunctionName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.OrganisationId))

                .Text(t => t
                    .Name(p => p.OrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Nested<Contact>(n => n
                    .Name(p => p.Contacts)
                    .IncludeInRoot()
                    .Properties(Contact.Mapping))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<PersonCapacity> Capacities { get; set; }
        public class PersonCapacity
        {
            public Guid PersonCapacityId { get; set; }
            public Guid CapacityId { get; set; }
            public string CapacityName { get; set; }
            public Guid OrganisationId { get; set; }
            public string OrganisationName { get; set; }
            public Guid? FunctionId { get; set; }
            public string FunctionName { get; set; }
            public List<Contact> Contacts { get; set; }
            public Period Validity { get; set; }

            public PersonCapacity(
                Guid personCapacityId,
                Guid capacityId,
                string capacityName,
                Guid organisationId,
                string organisationName,
                Guid? functionId,
                string functionName,
                List<Contact> contacts,
                Period validity)
            {
                PersonCapacityId = personCapacityId;
                CapacityId = capacityId;
                OrganisationId = organisationId;
                FunctionId = functionId;
                Contacts = contacts;
                Validity = validity;
                CapacityName = capacityName;
                OrganisationName = organisationName;
                FunctionName = functionName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<PersonCapacity> map) => map
                .Keyword(k => k
                    .Name(p => p.PersonCapacityId))

                .Keyword(k => k
                    .Name(p => p.CapacityId))

                .Text(t => t
                    .Name(p => p.CapacityName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.OrganisationId))

                .Text(t => t
                    .Name(p => p.OrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.FunctionId))

                .Text(t => t
                    .Name(p => p.FunctionName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Nested<Contact>(n => n
                    .Name(p => p.Contacts)
                    .IncludeInRoot()
                    .Properties(Contact.Mapping))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<PersonMandate> Mandates { get; set; }
        public class PersonMandate
        {
            public Guid BodyMandateId { get; set; }
            public Guid? DelegationAssignmentId { get; set; }
            public Guid BodyId { get; set; }
            public string BodyName { get; set; }
            public Guid? BodyOrganisationId { get; set; }
            public string BodyOrganisationName { get; set; }
            public Guid BodySeatId { get; set; }
            public string BodySeatName { get; set; }
            public string BodySeatNumber { get; set; }
            public bool PaidSeat { get; set; }
            public Period Validity { get; set; }

            public PersonMandate(
                Guid bodyMandateId,
                Guid? delegationAssignmentId,
                Guid bodyId,
                string bodyName,
                Guid? bodyOrganisationId,
                string bodyOrganisationName,
                Guid bodySeatId,
                string bodySeatName,
                string bodySeatNumber,
                bool paidSeat,
                Period validity)
            {
                BodyMandateId = bodyMandateId;
                DelegationAssignmentId = delegationAssignmentId;
                BodyId = bodyId;
                BodyName = bodyName;
                BodyOrganisationId = bodyOrganisationId;
                BodyOrganisationName = bodyOrganisationName;
                BodySeatId = bodySeatId;
                BodySeatName = bodySeatName;
                BodySeatNumber = bodySeatNumber;
                PaidSeat = paidSeat;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<PersonMandate> map) => map
                .Keyword(k => k
                    .Name(p => p.BodyMandateId))

                .Keyword(k => k
                    .Name(p => p.DelegationAssignmentId))

                .Keyword(k => k
                    .Name(p => p.BodyId))

                .Text(t => t
                    .Name(p => p.BodyName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.BodyOrganisationId))

                .Text(t => t
                    .Name(p => p.BodyOrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.BodySeatId))

                .Text(t => t
                    .Name(p => p.BodySeatName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Text(t => t
                    .Name(p => p.BodySeatNumber)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(b => b
                    .Name(p => p.PaidSeat))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }
    }
}
