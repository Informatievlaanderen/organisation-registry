namespace OrganisationRegistry.Api.Backoffice.Report.Responses
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class FormalFrameworkOrganisationBase
    {
        [ExcludeFromCsv]
        public Guid? ParentOrganisationId { get; set; }

        [Order]
        [DisplayName("Moeder entiteit")]
        public string ParentOrganisationName { get; set; }

        [ExcludeFromCsv]
        public Guid OrganisationId { get; set; }

        [Order]
        [DisplayName("Entiteit")]
        public string OrganisationName { get; set; }

        [Order]
        [DisplayName("Korte naam")]
        public string OrganisationShortName { get; set; }

        [Order]
        [DisplayName("OVO-nummer")]
        public string OrganisationOvoNumber { get; set; }

        [Order]
        [DisplayName("Data.Vlaanderen link")]
        public Uri DataVlaanderenOrganisationUri { get; set; }

        [Order]
        [DisplayName("Juridische vorm")]
        public string? LegalForm { get; set; }

        [Order]
        [DisplayName("Beleidsdomein")]
        public string? PolicyDomain { get; set; }

        [Order]
        [DisplayName("Bevoegde minister")]
        public string? ResponsibleMinister { get; set; }

        [Order]
        [DisplayName("Hoofdlocatie")]
        public string? MainLocation { get; set; }

        [Order]
        [DisplayName("Locatie")]
        public string? Location { get; set; }

        public FormalFrameworkOrganisationBase(
            OrganisationDocument document,
            ApiConfiguration @params,
            Guid formalFrameworkId,
            DateTime today)
        {
            var formalFramework = document.FormalFrameworks
                ?.Single(
                    x =>
                        x.FormalFrameworkId == formalFrameworkId &&
                        (x.Validity == null ||
                         x.Validity.OverlapsWith(today)));

            ParentOrganisationId = formalFramework?.ParentOrganisationId;
            ParentOrganisationName = formalFramework?.ParentOrganisationName;

            OrganisationId = document.Id;
            OrganisationName = document.Name;
            OrganisationShortName = document.ShortName;
            OrganisationOvoNumber = document.OvoNumber;

            DataVlaanderenOrganisationUri =
                new Uri(string.Format(@params.DataVlaanderenOrganisationUri, document.OvoNumber));

            LegalForm = document.OrganisationClassifications
                ?.SingleOrDefault(x => x.OrganisationClassificationTypeId == @params.LegalFormClassificationTypeId &&
                                       (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                                       (!x.Validity.End.HasValue || x.Validity.End.Value >= today))
                ?.OrganisationClassificationName;

            PolicyDomain = document.OrganisationClassifications
                ?.SingleOrDefault(x => x.OrganisationClassificationTypeId == @params.PolicyDomainClassificationTypeId &&
                                       (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                                       (!x.Validity.End.HasValue || x.Validity.End.Value >= today))
                ?.OrganisationClassificationName;

            ResponsibleMinister = document.OrganisationClassifications
                ?.SingleOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.ResponsibleMinisterClassificationTypeId &&
                    (!x.Validity.Start.HasValue || x.Validity.Start <= today) &&
                    (!x.Validity.End.HasValue || x.Validity.End >= today))
                ?.OrganisationClassificationName;

            MainLocation = document.Locations
                ?.SingleOrDefault(x => x.IsMainLocation &&
                                       (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                                       (!x.Validity.End.HasValue || x.Validity.End.Value >= today))
                ?.FormattedAddress;

            Location = document.Locations
                ?.FirstOrDefault(x => !x.IsMainLocation &&
                                      (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                                      (!x.Validity.End.HasValue || x.Validity.End.Value >= today))
                ?.FormattedAddress;
        }
    }
}
