namespace OrganisationRegistry.Api.Report.Responses
{
    using Configuration;
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

    public class ClassificationOrganisation
    {
        [ExcludeFromCsv]
        public Guid? ParentOrganisationId { get; set; }

        [DisplayName("Moeder entiteit")]
        public string ParentOrganisationName { get; set; }

        [ExcludeFromCsv]
        public Guid OrganisationId { get; set; }

        [DisplayName("Entiteit")]
        public string OrganisationName { get; set; }

        [DisplayName("Korte naam")]
        public string OrganisationShortName { get; set; }

        [DisplayName("Franse naam")]
        public string OrganisationNameFrench { get; set; }

        [DisplayName("Engelse naam")]
        public string OrganisationNameEnglish { get; set; }

        [DisplayName("Duitse naam")]
        public string OrganisationNameGerman { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="params"></param>
        public ClassificationOrganisation(
            OrganisationDocument document,
            ApiConfiguration @params)
        {
            var parent = document
                .Parents?
                .FirstOrDefault(
                    x => x.Validity == null ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now));

            ParentOrganisationId = parent?.ParentOrganisationId;
            ParentOrganisationName = parent?.ParentOrganisationName;

            OrganisationId = document.Id;
            OrganisationName = document.Name;
            OrganisationShortName = document.ShortName;

            OrganisationNameFrench = document.Labels
                ?.FirstOrDefault(x => x.LabelTypeId == @params.FrenchLabelTypeId)
                ?.Value;

            OrganisationNameEnglish = document.Labels
                ?.FirstOrDefault(x => x.LabelTypeId == @params.EnglishLabelTypeId)
                ?.Value;

            OrganisationNameGerman = document.Labels
                ?.FirstOrDefault(x => x.LabelTypeId == @params.GermanLabelTypeId)
                ?.Value;
        }

        /// <summary>
        /// Scroll through all <see cref="OrganisationDocument"/> with a matching (and active) <see cref="OrganisationClassification"/>, and return entire dataset
        /// </summary>
        /// <param name="client"></param>
        /// <param name="organisationClassificationId"></param>
        /// <param name="scrollSize"></param>
        /// <param name="scrollTimeout"></param>
        /// <returns></returns>
        public static async Task<IList<OrganisationDocument>> Search(
            IElasticClient client,
            Guid organisationClassificationId,
            int scrollSize,
            string scrollTimeout)
        {
            var results = new List<OrganisationDocument>();

            var scroll = await client.SearchAsync<OrganisationDocument>(s => s
                .From(0)
                .Size(scrollSize)
                .Query(q => q
                    .Match(match => match
                        .Field(f => f.OrganisationClassifications.Single().OrganisationClassificationId)
                        .Query(organisationClassificationId.ToString())))
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
        /// Map each <see cref="OrganisationDocument"/> to a <see cref="ClassificationOrganisation"/>
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="organisationClassificationId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static IEnumerable<ClassificationOrganisation> Map(
            IEnumerable<OrganisationDocument> documents,
            Guid organisationClassificationId,
            ApiConfiguration @params)
        {
            var classificationOrganisations = new List<ClassificationOrganisation>();

            foreach (var document in documents)
            {
                var classifications = document
                    .OrganisationClassifications?
                    .Where(x =>
                        x.OrganisationClassificationId == organisationClassificationId &&
                        (x.Validity == null ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now)))
                    .ToList();

                if (classifications == null || !classifications.Any())
                    continue;

                classificationOrganisations.Add(new ClassificationOrganisation(document, @params));
            }

            return classificationOrganisations;
        }

        /// <summary>
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<ClassificationOrganisation> Sort(
            IEnumerable<ClassificationOrganisation> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.OrganisationName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "organisationnamefrench":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationNameFrench)
                        : results.OrderByDescending(x => x.OrganisationNameFrench);
                case "organisationnameenglish":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationNameEnglish)
                        : results.OrderByDescending(x => x.OrganisationNameEnglish);
                case "organisationnamegerman":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationNameGerman)
                        : results.OrderByDescending(x => x.OrganisationNameGerman);
                case "organisationname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationName)
                        : results.OrderByDescending(x => x.OrganisationName);
                default:
                    return results.OrderBy(x => x.OrganisationName);
            }
        }
    }
}
