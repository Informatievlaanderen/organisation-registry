namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using Nest;
    using System;
    using System.Collections.Generic;
    using Bodies;
    using Common;
    using Configuration;
    using Newtonsoft.Json;

    public class OrganisationDocument : IDocument
    {
        public int ChangeId { get; set; }
        public DateTimeOffset ChangeTime { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string OvoNumber { get; set; }
        public string ShortName { get; set; }
        public Period Validity { get; set; }
        public string Description { get; set; }
        public string KboNumber { get; set; }
        public bool? ShowOnVlaamseOverheidSites { get; set; }

        public OrganisationDocument() { }

        public static TypeMappingDescriptor<OrganisationDocument> Mapping(TypeMappingDescriptor<OrganisationDocument> map) => map
            .Properties(ps => ps
                .Number(n => n
                    .Name(p => p.ChangeId))

                .Date(d => d
                    .Name(p => p.ChangeTime)
                    .Format(ElasticSearchConfiguration.DateFormat))

                .Keyword(k => k
                    .Name(p => p.Id))

                .Text(t => t
                    .Name(p => p.Name)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.OvoNumber))

                .Text(t => t
                    .Name(p => p.ShortName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping))

                .Text(t => t
                    .Name(p => p.Description)
                    .Analyzer("dutch"))

                .Keyword(k => k
                    .Name(p => p.KboNumber))

                .Boolean(b => b
                    .Name(p => p.ShowOnVlaamseOverheidSites))

                .Nested<Purpose>(n => n
                    .Name(p => p.Purposes)
                    .IncludeInRoot()
                    .Properties(Purpose.Mapping))

                .Nested<OrganisationLabel>(n => n
                    .Name(p => p.Labels)
                    .IncludeInRoot()
                    .Properties(OrganisationLabel.Mapping))

                .Nested<OrganisationKey>(n => n
                    .Name(p => p.Keys)
                    .IncludeInRoot()
                    .Properties(OrganisationKey.Mapping))

                .Nested<OrganisationContact>(n => n
                    .Name(p => p.Contacts)
                    .IncludeInRoot()
                    .Properties(OrganisationContact.Mapping))

                .Nested<OrganisationOrganisationClassification>(n => n
                    .Name(p => p.OrganisationClassifications)
                    .IncludeInRoot()
                    .Properties(OrganisationOrganisationClassification.Mapping))

                .Nested<OrganisationFunction>(n => n
                    .Name(p => p.Functions)
                    .IncludeInRoot()
                    .Properties(OrganisationFunction.Mapping))

                .Nested<OrganisationCapacity>(n => n
                    .Name(p => p.Capacities)
                    .IncludeInRoot()
                    .Properties(OrganisationCapacity.Mapping))

                .Nested<OrganisationParent>(n => n
                    .Name(p => p.Parents)
                    .IncludeInRoot()
                    .Properties(OrganisationParent.Mapping))

                .Nested<OrganisationFormalFramework>(n => n
                    .Name(p => p.FormalFrameworks)
                    .IncludeInRoot()
                    .Properties(OrganisationFormalFramework.Mapping))

                .Nested<OrganisationBuilding>(n => n
                    .Name(p => p.Buildings)
                    .IncludeInRoot()
                    .Properties(OrganisationBuilding.Mapping))

                .Nested<OrganisationLocation>(n => n
                    .Name(p => p.Locations)
                    .IncludeInRoot()
                    .Properties(OrganisationLocation.Mapping))

                .Nested<OrganisationBody>(n => n
                    .Name(p => p.Bodies)
                    .IncludeInRoot()
                    .Properties(OrganisationBody.Mapping))

                .Nested<OrganisationBankAccount>(n => n
                    .Name(p => p.BankAccounts)
                    .IncludeInRoot()
                    .Properties(OrganisationBankAccount.Mapping))

                .Nested<OrganisationOpeningHour>(n => n
                    .Name(p => p.OpeningHours)
                    .IncludeInRoot()
                    .Properties(OrganisationOpeningHour.Mapping))
            );

        public List<Purpose> Purposes { get; set; }
        public class Purpose
        {
            public Guid PurposeId { get; set; }
            public string PurposeName { get; set; }

            protected Purpose() { }

            public Purpose(Guid purposeId, string purposeName)
            {
                PurposeId = purposeId;
                PurposeName = purposeName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<Purpose> map) => map
                .Keyword(k => k
                    .Name(p => p.PurposeId))

                .Text(t => t
                    .Name(p => p.PurposeName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))));
        }

        public List<OrganisationLabel> Labels { get; set; }
        public class OrganisationLabel
        {
            public Guid OrganisationLabelId { get; set; }
            public Guid LabelTypeId { get; set; }
            public string LabelTypeName { get; set; }
            public string Value { get; set; }
            public Period Validity { get; set; }

            protected OrganisationLabel() { }

            public OrganisationLabel(
                Guid organisationLabelId,
                Guid labelTypeId,
                string labelTypeName,
                string value,
                Period validity)
            {
                OrganisationLabelId = organisationLabelId;
                LabelTypeId = labelTypeId;
                LabelTypeName = labelTypeName;
                Value = value;
                Validity = validity;
            }


            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationLabel> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationLabelId))

                .Keyword(k => k
                    .Name(p => p.LabelTypeId))

                .Text(t => t
                    .Name(p => p.LabelTypeName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Text(t => t
                    .Name(p => p.Value)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationKey> Keys { get; set; }
        public class OrganisationKey
        {
            public Guid OrganisationKeyId { get; set; }
            public Guid KeyTypeId { get; set; }
            public string KeyTypeName { get; set; }
            public string Value { get; set; }
            public Period Validity { get; set; }

            protected OrganisationKey() { }

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

        public List<OrganisationContact> Contacts { get; set; }
        public class OrganisationContact
        {
            public Guid OrganisationContactId { get; set; }
            public Guid ContactTypeId { get; set; }
            public string ContactTypeName { get; set; }
            public string Value { get; set; }
            public Period Validity { get; set; }

            protected OrganisationContact() { }

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

        public List<OrganisationOrganisationClassification> OrganisationClassifications { get; set; }
        public class OrganisationOrganisationClassification
        {
            public Guid OrganisationOrganisationClassificationId { get; set; }
            public Guid OrganisationClassificationTypeId { get; set; }
            public string OrganisationClassificationTypeName { get; set; }
            public Guid OrganisationClassificationId { get; set; }
            public string OrganisationClassificationName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationOrganisationClassification() { }

            public OrganisationOrganisationClassification(
                Guid organisationOrganisationClassificationId,
                Guid organisationClassificationTypeId,
                string organisationClassificationTypeName,
                Guid organisationClassificationId,
                string organisationClassificationName,
                Period validity)
            {
                OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
                OrganisationClassificationTypeId = organisationClassificationTypeId;
                OrganisationClassificationId = organisationClassificationId;
                Validity = validity;
                OrganisationClassificationTypeName = organisationClassificationTypeName;
                OrganisationClassificationName = organisationClassificationName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationOrganisationClassification> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationOrganisationClassificationId))

                .Keyword(k => k
                    .Name(p => p.OrganisationClassificationTypeId))

                .Text(t => t
                    .Name(p => p.OrganisationClassificationTypeName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.OrganisationClassificationId))

                .Text(t => t
                    .Name(p => p.OrganisationClassificationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationFunction> Functions { get; set; }
        public class OrganisationFunction
        {
            public Guid OrganisationFunctionId { get; set; }
            public Guid FunctionId { get; set; }
            public string FunctionName { get; }
            public Guid PersonId { get; set; }
            public string PersonName { get; }
            public List<Contact> Contacts { get; set; }
            public Period Validity { get; set; }

            protected OrganisationFunction() { }

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

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationFunction> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationFunctionId))

                .Keyword(k => k
                    .Name(p => p.FunctionId))

                .Text(t => t
                    .Name(p => p.FunctionName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.PersonId))

                .Text(t => t
                    .Name(p => p.PersonName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Nested<Contact>(n => n
                    .Name(p => p.Contacts)
                    .IncludeInRoot()
                    .Properties(Contact.Mapping))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationRelation> Relations { get; set; }
        public class OrganisationRelation
        {
            public Guid OrganisationRelationId { get; set; }
            public Guid RelationId { get; set; }
            public string RelationName { get; }
            public Guid RelatedOrganisationId { get; set; }
            public string RelatedOrganisationOvoNumber { get; }
            public string RelatedOrganisationName { get; }
            public Period Validity { get; set; }

            protected OrganisationRelation() { }

            public OrganisationRelation(
                Guid organisationRelationId,
                Guid relationId,
                string relationName,
                Guid relatedOrganisationId,
                string relatedOrganisationOvoNumber,
                string relatedOrganisationName,
                Period validity)
            {
                OrganisationRelationId = organisationRelationId;
                RelationId = relationId;
                RelationName = relationName;
                RelatedOrganisationId = relatedOrganisationId;
                RelatedOrganisationOvoNumber = relatedOrganisationOvoNumber;
                RelatedOrganisationName = relatedOrganisationName;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationRelation> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationRelationId))

                .Keyword(k => k
                    .Name(p => p.RelationId))

                .Text(t => t
                    .Name(p => p.RelationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.RelatedOrganisationId))

                .Keyword(k => k
                    .Name(p => p.RelatedOrganisationOvoNumber))

                .Text(t => t
                    .Name(p => p.RelatedOrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationCapacity> Capacities { get; set; }
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

            protected OrganisationCapacity() { }

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

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationCapacity> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationCapacityId))

                .Keyword(k => k
                    .Name(p => p.CapacityId))

                .Text(t => t
                    .Name(p => p.CapacityName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.PersonId))

                .Text(t => t
                    .Name(p => p.PersonName)
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

        public List<OrganisationParent> Parents { get; set; }
        public class OrganisationParent
        {
            public Guid OrganisationOrganisationParentId { get; set; }
            public Guid ParentOrganisationId { get; set; }
            public string ParentOrganisationName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationParent() { }

            public OrganisationParent(
                Guid organisationOrganisationParentId,
                Guid parentOrganisationId,
                string parentOrganisationName,
                Period validity)
            {
                OrganisationOrganisationParentId = organisationOrganisationParentId;
                ParentOrganisationId = parentOrganisationId;
                Validity = validity;
                ParentOrganisationName = parentOrganisationName;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationParent> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationOrganisationParentId))

                .Keyword(k => k
                    .Name(p => p.ParentOrganisationId))

                .Text(t => t
                    .Name(p => p.ParentOrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationFormalFramework> FormalFrameworks { get; set; }
        public class OrganisationFormalFramework
        {
            public Guid OrganisationFormalFrameworkId { get; set; }
            public Guid FormalFrameworkId { get; set; }
            public string FormalFrameworkName { get; set; }
            public Guid ParentOrganisationId { get; set; }
            public string ParentOrganisationName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationFormalFramework() { }

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

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationFormalFramework> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationFormalFrameworkId))

                .Keyword(k => k
                    .Name(p => p.FormalFrameworkId))

                .Text(t => t
                    .Name(p => p.FormalFrameworkName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Keyword(k => k
                    .Name(p => p.ParentOrganisationId))

                .Text(t => t
                    .Name(p => p.ParentOrganisationName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationBuilding> Buildings { get; set; }
        public class OrganisationBuilding
        {
            public Guid OrganisationBuildingId { get; set; }
            public Guid BuildingId { get; set; }
            public string BuildingName { get; set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public bool IsMainBuilding { get; set; }
            public Period Validity { get; set; }

            protected OrganisationBuilding() { }

            public OrganisationBuilding(
                Guid organisationBuildingId,
                Guid buildingId,
                string buildingName,
                bool isMainBuilding,
                Period validity)
            {
                OrganisationBuildingId = organisationBuildingId;
                BuildingId = buildingId;
                BuildingName = buildingName;
                IsMainBuilding = isMainBuilding;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBuilding> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationBuildingId))

                .Keyword(k => k
                    .Name(p => p.BuildingId))

                .Text(t => t
                    .Name(p => p.BuildingName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(t => t
                    .Name(p => p.IsMainBuilding))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationLocation> Locations { get; set; }
        public class OrganisationLocation
        {
            public Guid OrganisationLocationId { get; set; }
            public Guid LocationId { get; set; }
            public string FormattedAddress { get; set; }
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public bool IsMainLocation { get; set; }
            public Guid? LocationTypeId { get; set; }
            public string LocationTypeName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationLocation() { }

            public OrganisationLocation(
                Guid organisationLocationId,
                Guid locationId,
                string formattedAddress,
                bool isMainLocation,
                Guid? locationTypeId,
                string locationTypeName,
                Period validity)
            {
                OrganisationLocationId = organisationLocationId;
                LocationId = locationId;
                IsMainLocation = isMainLocation;
                LocationTypeId = locationTypeId;
                LocationTypeName = locationTypeName;
                Validity = validity;
                FormattedAddress = formattedAddress;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationLocation> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationLocationId))

                .Keyword(k => k
                    .Name(p => p.LocationId))

                .Text(t => t
                    .Name(p => p.FormattedAddress)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(t => t
                    .Name(p => p.IsMainLocation))

                .Keyword(k => k
                    .Name(p => p.LocationTypeId))

                .Text(t => t
                    .Name(p => p.LocationTypeName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationBody> Bodies { get; set; }
        public class OrganisationBody
        {
            public Guid BodyOrganisationId { get; set; }
            public Guid BodyId { get; set; }
            public string BodyName { get; set; }
            public Period Validity { get; set; }

            protected OrganisationBody() { }

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

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBody> map) => map
                .Keyword(k => k
                    .Name(p => p.BodyOrganisationId))

                .Keyword(k => k
                    .Name(p => p.BodyId))

                .Text(t => t
                    .Name(p => p.BodyName)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationBankAccount> BankAccounts { get; set; }
        public class OrganisationBankAccount
        {
            public Guid OrganisationBankAccountId { get; set; }
            public string BankAccountNumber { get; set; }
            public bool IsIban { get; set; }
            public string Bic { get; set; }
            public bool IsBic { get; set; }
            public Period Validity { get; set; }

            protected OrganisationBankAccount() { }

            public OrganisationBankAccount(
                Guid organisationBankAccountId,
                string bankAccountNumber,
                bool isIban,
                string bic,
                bool isBic,
                Period validity)
            {
                OrganisationBankAccountId = organisationBankAccountId;
                BankAccountNumber = bankAccountNumber;
                IsIban = isIban;
                Bic = bic;
                IsBic = isBic;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationBankAccount> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationBankAccountId))

                .Text(t => t
                    .Name(p => p.BankAccountNumber)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(t => t
                    .Name(p => p.IsIban))

                .Text(t => t
                    .Name(p => p.Bic)
                    .Fields(x => x.Keyword(y => y.Name("keyword"))))

                .Boolean(t => t
                    .Name(p => p.IsBic))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }

        public List<OrganisationOpeningHour> OpeningHours { get; set; }
        public class OrganisationOpeningHour
        {
            public Guid OrganisationOpeningHourId { get; set; }

            public TimeSpan Opens { get; set; }

            public TimeSpan Closes { get; set; }

            public DayOfWeek? DayOfWeek { get; set; }

            public Period Validity { get; set; }

            protected OrganisationOpeningHour() { }

            public OrganisationOpeningHour(
                Guid organisationOpeningHourId,
                TimeSpan opens,
                TimeSpan closes,
                DayOfWeek? dayOfWeek,
                Period validity)
            {
                OrganisationOpeningHourId = organisationOpeningHourId;
                Opens = opens;
                Closes = closes;
                DayOfWeek = dayOfWeek;
                Validity = validity;
            }

            public static IPromise<IProperties> Mapping(PropertiesDescriptor<OrganisationOpeningHour> map) => map
                .Keyword(k => k
                    .Name(p => p.OrganisationOpeningHourId))

                .Keyword(k => k
                    .Name(p => p.Opens))

                .Keyword(k => k
                    .Name(p => p.Closes))

                .Keyword(k => k
                    .Name(p => p.DayOfWeek))

                .Object<Period>(o => o
                    .Name(p => p.Validity)
                    .Properties(Period.Mapping));
        }
    }
}
