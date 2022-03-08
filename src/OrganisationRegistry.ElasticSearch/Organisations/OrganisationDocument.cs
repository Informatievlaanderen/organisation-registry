namespace OrganisationRegistry.ElasticSearch.Organisations
{
    using Osc;
    using System;
    using System.Collections.Generic;
    using Common;

    public partial class OrganisationDocument : IDocument
    {
        public OrganisationDocument()
        {
            Name = string.Empty;
            OvoNumber = string.Empty;
            ShortName = string.Empty;
            Description = string.Empty;
            KboNumber = string.Empty;
            Purposes = new List<Purpose>();
            Labels = new List<OrganisationLabel>();
            Keys = new List<OrganisationKey>();
            Contacts = new List<OrganisationContact>();
            OrganisationClassifications = new List<OrganisationOrganisationClassification>();
            Functions = new List<OrganisationFunction>();
            Relations = new List<OrganisationRelation>();
            Capacities = new List<OrganisationCapacity>();
            Parents = new List<OrganisationParent>();
            FormalFrameworks = new List<OrganisationFormalFramework>();
            Buildings = new List<OrganisationBuilding>();
            Locations = new List<OrganisationLocation>();
            Bodies = new List<OrganisationBody>();
            BankAccounts = new List<OrganisationBankAccount>();
            OpeningHours = new List<OrganisationOpeningHour>();
            Regulations = new List<OrganisationRegulation>();
            Validity = Period.Infinite();
        }

        public string OvoNumber { get; set; }
        public string ShortName { get; set; }
        public string? Article { get; set; }
        public Period Validity { get; set; }
        public Period? OperationalValidity { get; set; }
        public string Description { get; set; }
        public string KboNumber { get; set; }
        public bool? ShowOnVlaamseOverheidSites { get; set; }
        public string? ManagedBy { get; set; }

        public List<Purpose> Purposes { get; set; }
        public List<OrganisationLabel> Labels { get; set; }
        public List<OrganisationKey> Keys { get; set; }
        public List<OrganisationContact> Contacts { get; set; }
        public List<OrganisationOrganisationClassification> OrganisationClassifications { get; set; }
        public List<OrganisationFunction> Functions { get; set; }
        public List<OrganisationRelation> Relations { get; set; }
        public List<OrganisationCapacity> Capacities { get; set; }
        public List<OrganisationParent> Parents { get; set; }
        public List<OrganisationFormalFramework> FormalFrameworks { get; set; }
        public List<OrganisationBuilding> Buildings { get; set; }
        public List<OrganisationLocation> Locations { get; set; }
        public List<OrganisationBody> Bodies { get; set; }
        public List<OrganisationBankAccount> BankAccounts { get; set; }
        public List<OrganisationOpeningHour> OpeningHours { get; set; }
        public List<OrganisationRegulation> Regulations { get; set; }
        public int ChangeId { get; set; }
        public DateTimeOffset ChangeTime { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }

        public static TypeMappingDescriptor<OrganisationDocument> Mapping(
            TypeMappingDescriptor<OrganisationDocument> map)
            => map
                .Properties(
                    ps => ps
                        .Number(
                            n => n
                                .Name(p => p.ChangeId))
                        .Date(
                            d => d
                                .Name(p => p.ChangeTime))
                        .Keyword(
                            k => k
                                .Name(p => p.Id))
                        .Text(
                            t => t
                                .Name(p => p.Name)
                                .Fields(x => x.Keyword(y => y.Name("keyword"))))
                        .Keyword(
                            k => k
                                .Name(p => p.OvoNumber))
                        .Text(
                            t => t
                                .Name(p => p.ShortName)
                                .Fields(x => x.Keyword(y => y.Name("keyword"))))
                        .Text(
                            t => t
                                .Name(p => p.Article)
                                .Fields(x => x.Keyword(y => y.Name("keyword"))))
                        .Object<Period>(
                            o => o
                                .Name(p => p.Validity)
                                .Properties(Period.Mapping))
                        .Object<Period>(
                            o => o
                                .Name(p => p.OperationalValidity)
                                .Properties(Period.Mapping))
                        .Text(
                            t => t
                                .Name(p => p.Description)
                                .Analyzer("dutch"))
                        .Keyword(
                            k => k
                                .Name(p => p.KboNumber))
                        .Boolean(
                            b => b
                                .Name(p => p.ShowOnVlaamseOverheidSites))
                        .Keyword(
                            k => k
                                .Name(p => p.ManagedBy))
                        .Nested<Purpose>(
                            n => n
                                .Name(p => p.Purposes)
                                .IncludeInRoot()
                                .Properties(Purpose.Mapping))
                        .Nested<OrganisationLabel>(
                            n => n
                                .Name(p => p.Labels)
                                .IncludeInRoot()
                                .Properties(OrganisationLabel.Mapping))
                        .Nested<OrganisationKey>(
                            n => n
                                .Name(p => p.Keys)
                                .IncludeInRoot()
                                .Properties(OrganisationKey.Mapping))
                        .Nested<OrganisationContact>(
                            n => n
                                .Name(p => p.Contacts)
                                .IncludeInRoot()
                                .Properties(OrganisationContact.Mapping))
                        .Nested<OrganisationOrganisationClassification>(
                            n => n
                                .Name(p => p.OrganisationClassifications)
                                .IncludeInRoot()
                                .Properties(OrganisationOrganisationClassification.Mapping))
                        .Nested<OrganisationFunction>(
                            n => n
                                .Name(p => p.Functions)
                                .IncludeInRoot()
                                .Properties(OrganisationFunction.Mapping))
                        .Nested<OrganisationCapacity>(
                            n => n
                                .Name(p => p.Capacities)
                                .IncludeInRoot()
                                .Properties(OrganisationCapacity.Mapping))
                        .Nested<OrganisationParent>(
                            n => n
                                .Name(p => p.Parents)
                                .IncludeInRoot()
                                .Properties(OrganisationParent.Mapping))
                        .Nested<OrganisationFormalFramework>(
                            n => n
                                .Name(p => p.FormalFrameworks)
                                .IncludeInRoot()
                                .Properties(OrganisationFormalFramework.Mapping))
                        .Nested<OrganisationBuilding>(
                            n => n
                                .Name(p => p.Buildings)
                                .IncludeInRoot()
                                .Properties(OrganisationBuilding.Mapping))
                        .Nested<OrganisationLocation>(
                            n => n
                                .Name(p => p.Locations)
                                .IncludeInRoot()
                                .Properties(OrganisationLocation.Mapping))
                        .Nested<OrganisationBody>(
                            n => n
                                .Name(p => p.Bodies)
                                .IncludeInRoot()
                                .Properties(OrganisationBody.Mapping))
                        .Nested<OrganisationBankAccount>(
                            n => n
                                .Name(p => p.BankAccounts)
                                .IncludeInRoot()
                                .Properties(OrganisationBankAccount.Mapping))
                        .Nested<OrganisationOpeningHour>(
                            n => n
                                .Name(p => p.OpeningHours)
                                .IncludeInRoot()
                                .Properties(OrganisationOpeningHour.Mapping))
                        .Nested<OrganisationRegulation>(
                            n => n
                                .Name(p => p.Regulations)
                                .IncludeInRoot()
                                .Properties(OrganisationRegulation.Mapping))
                );
    }
}
