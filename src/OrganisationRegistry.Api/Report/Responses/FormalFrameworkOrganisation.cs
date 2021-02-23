namespace OrganisationRegistry.Api.Report.Responses
{
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using Nest;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SortOrder = Infrastructure.Search.Sorting.SortOrder;

    public class FormalFrameworkOrganisation
    {
        /// <summary>
        /// Scroll through all <see cref="OrganisationDocument"/> with a matching (and active) <see cref="FormalFramework"/>, and return entire dataset
        /// </summary>
        /// <param name="client"></param>
        /// <param name="formalFrameworkId"></param>
        /// <param name="scrollSize"></param>
        /// <param name="scrollTimeout"></param>
        /// <returns></returns>
        public static async Task<IList<OrganisationDocument>> Search(
            IElasticClient client,
            Guid formalFrameworkId,
            int scrollSize,
            string scrollTimeout)
        {
            var results = new List<OrganisationDocument>();

            var scroll = await client.SearchAsync<OrganisationDocument>(s => s
                .From(0)
                .Size(scrollSize)
                .Query(q => q
                    .Match(match => match
                        .Field(f => f.FormalFrameworks.Single().FormalFrameworkId)
                        .Query(formalFrameworkId.ToString())))
                .Scroll(scrollTimeout));

            if (scroll.IsValid)
                results.AddRange(scroll.Documents);

            while (scroll.Documents.Any())
            {
                scroll = await client.ScrollAsync<OrganisationDocument>(scrollTimeout, scroll.ScrollId);

                if (scroll.IsValid)
                    results.AddRange(scroll.Documents);
            }

            return results;
        }

        /// <summary>
        /// Map each <see cref="OrganisationDocument"/> to a <see cref="FormalFrameworkOrganisationBase"/>
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="formalFrameworkId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkOrganisationBase> Map(
            IEnumerable<OrganisationDocument> documents,
            Guid formalFrameworkId,
            ApiConfiguration @params)
        {
            var formalFrameworkOrganisations = new List<FormalFrameworkOrganisationBase>();

            foreach (var document in documents)
            {
                var formalFrameworks = document
                    .FormalFrameworks?
                    .Where(x =>
                        x.FormalFrameworkId == formalFrameworkId &&
                        (x.Validity == null ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now)))
                    .ToList();

                if (formalFrameworks == null || !formalFrameworks.Any())
                    continue;

                formalFrameworkOrganisations.Add(
                    new FormalFrameworkOrganisationBase(
                        document,
                        @params,
                        formalFrameworkId,
                        DateTime.Now));
            }

            return formalFrameworkOrganisations;
        }

        /// <summary>
        /// Map each <see cref="OrganisationDocument"/> to a <see cref="FormalFrameworkOrganisationBase"/>
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="formalFrameworkId"></param>
        /// <param name="params"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkOrganisationExtended> MapExtended(
            IEnumerable<OrganisationDocument> documents,
            Guid formalFrameworkId,
            ApiConfiguration @params,
            DateTime today)
        {
            var formalFrameworkOrganisations = new List<FormalFrameworkOrganisationExtended>();

            foreach (var document in documents)
            {
                var formalFrameworks = document
                    .FormalFrameworks?
                    .Where(x =>
                        x.FormalFrameworkId == formalFrameworkId &&
                        (x.Validity == null ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= today)))
                    .ToList();

                if (formalFrameworks == null || !formalFrameworks.Any())
                    continue;

                formalFrameworkOrganisations.Add(
                    new FormalFrameworkOrganisationExtended(
                        document,
                        @params,
                        formalFrameworkId,
                        today));
            }

            return formalFrameworkOrganisations;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> Sort<T>(
            IEnumerable<T> results,
            SortingHeader sortingHeader) where T: FormalFrameworkOrganisationBase
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.OrganisationName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "legalform":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.LegalForm)
                        : results.OrderByDescending(x => x.LegalForm);
                case "policydomain":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.PolicyDomain)
                        : results.OrderByDescending(x => x.PolicyDomain);
                case "responsibleminister":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.ResponsibleMinister)
                        : results.OrderByDescending(x => x.ResponsibleMinister);
                case "organisationname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationName)
                        : results.OrderByDescending(x => x.OrganisationName);
                default:
                    return results.OrderBy(x => x.OrganisationName);
            }
        }
    }

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
        public string LegalForm { get; set; }

        [Order]
        [DisplayName("Beleidsdomein")]
        public string PolicyDomain { get; set; }

        [Order]
        [DisplayName("Bevoegde minister")]
        public string ResponsibleMinister { get; set; }

        [Order]
        [DisplayName("Hoofdlocatie")]
        public string MainLocation { get; set; }

        [Order]
        [DisplayName("Locatie")]
        public string Location { get; set; }

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
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= today)));

            ParentOrganisationId = formalFramework?.ParentOrganisationId;
            ParentOrganisationName = formalFramework?.ParentOrganisationName;

            OrganisationId = document.Id;
            OrganisationName = document.Name;
            OrganisationShortName = document.ShortName;
            OrganisationOvoNumber = document.OvoNumber;

            DataVlaanderenOrganisationUri =
                new Uri(string.Format(@params.DataVlaanderenOrganisationUri, document.OvoNumber));

            LegalForm = document.OrganisationClassifications
                ?.FirstOrDefault(x => x.OrganisationClassificationTypeId == @params.LegalFormClassificationTypeId)
                ?.OrganisationClassificationName;

            PolicyDomain = document.OrganisationClassifications
                ?.FirstOrDefault(x => x.OrganisationClassificationTypeId == @params.PolicyDomainClassificationTypeId)
                ?.OrganisationClassificationName;

            ResponsibleMinister = document.OrganisationClassifications
                ?.Where(x =>
                    (!x.Validity.Start.HasValue || x.Validity.Start <= today) &&
                    (!x.Validity.End.HasValue || x.Validity.End >= today))
                .SingleOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.ResponsibleMinisterClassificationTypeId)
                ?.OrganisationClassificationName;

            MainLocation = document.Locations
                ?.FirstOrDefault(x => x.IsMainLocation)
                ?.FormattedAddress;

            Location = document.Locations
                ?.FirstOrDefault(x => !x.IsMainLocation)
                ?.FormattedAddress;
        }
    }

    public class FormalFrameworkOrganisationExtended : FormalFrameworkOrganisationBase
    {
        [Order]
        [DisplayName("INR")]
        public string INR { get; set; }

        [Order]
        [DisplayName("KBO")]
        public string KBO { get; set; }

        [Order]
        [DisplayName("Orafin")]
        public string Orafin { get; set; }

        [Order]
        [DisplayName("Vlimpers")]
        public string Vlimpers { get; set; }

        [Order]
        [DisplayName("Vlimpers_kort")]
        public string VlimpersKort { get; set; }

        [Order]
        [DisplayName("Bestuursniveau")]
        public string Bestuursniveau { get; set; }

        [Order]
        [DisplayName("Categorie")]
        public string Categorie { get; set; }

        [Order]
        [DisplayName("Entiteitsvorm")]
        public string Entiteitsvorm { get; set; }

        [Order]
        [DisplayName("ESR Klasse toezichthoudende overheid")]
        public string ESRKlasseToezichthoudendeOverheid { get; set; }

        [Order]
        [DisplayName("ESR Sector")]
        public string ESRSector { get; set; }

        [Order]
        [DisplayName("ESR Toezichthoudende overheid")]
        public string ESRToezichthoudendeOverheid { get; set; }

        [Order]
        [DisplayName("Uitvoerend niveau")]
        public string UitvoerendNiveau { get; set; }

        [Order]
        [DisplayName("Geldig vanaf")]
        public DateTime? ValidFrom { get; set; }

        [Order]
        [DisplayName("Geldig tot")]
        public DateTime? ValidTo { get; set; }

        public FormalFrameworkOrganisationExtended(
            OrganisationDocument document,
            ApiConfiguration @params,
            Guid formalFrameworkId,
            DateTime today)
            : base(document, @params, formalFrameworkId, today)
        {
            INR = document.Keys
                ?.FirstOrDefault(x =>
                    x.KeyTypeId == @params.INR_KeyTypeId)
                ?.Value;
            KBO = document.Keys
                ?.FirstOrDefault(x =>
                    x.KeyTypeId == @params.KBO_KeyTypeId)
                ?.Value;
            Orafin = document.Keys
                ?.FirstOrDefault(x =>
                    x.KeyTypeId == @params.Orafin_KeyTypeId)
                ?.Value;
            Vlimpers = document.Keys
                ?.FirstOrDefault(x =>
                    x.KeyTypeId == @params.Vlimpers_KeyTypeId)
                ?.Value;
            VlimpersKort = document.Keys
                ?.FirstOrDefault(x =>
                    x.KeyTypeId == @params.VlimpersKort_KeyTypeId)
                ?.Value;

            Bestuursniveau = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.Bestuursniveau_ClassificationTypeId)
                ?.OrganisationClassificationName;
            Categorie = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.Categorie_ClassificationTypeId)
                ?.OrganisationClassificationName;
            Entiteitsvorm = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.Entiteitsvorm_ClassificationTypeId)
                ?.OrganisationClassificationName;
            ESRKlasseToezichthoudendeOverheid = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId ==
                    @params.ESRKlasseToezichthoudendeOverheid_ClassificationTypeId)
                ?.OrganisationClassificationName;
            ESRSector = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.ESRSector_ClassificationTypeId)
                ?.OrganisationClassificationName;
            ESRToezichthoudendeOverheid = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.ESRToezichthoudendeOverheid_ClassificationTypeId)
                ?.OrganisationClassificationName;
            UitvoerendNiveau = document.OrganisationClassifications
                ?.FirstOrDefault(x =>
                    x.OrganisationClassificationTypeId == @params.UitvoerendNiveau_ClassificationTypeId)
                ?.OrganisationClassificationName;

            ValidFrom = document.Validity.Start;

            ValidTo = document.Validity.End;
        }
    }
}
