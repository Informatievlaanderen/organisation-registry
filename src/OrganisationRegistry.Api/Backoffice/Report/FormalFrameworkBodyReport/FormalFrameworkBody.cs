namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkBodyReport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using ElasticSearch.Bodies;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Osc;
    using SortOrder = Infrastructure.Search.Sorting.SortOrder;

    public class FormalFrameworkBody
    {
        [ExcludeFromCsv]
        public Guid BodyId { get; set; }

        [DisplayName("Orgaan")]
        public string BodyName { get; set; }

        [DisplayName("Korte naam")]
        public string BodyShortName { get; set; }

        [DisplayName("Code")]
        public string BodyNumber { get; set; }

        [ExcludeFromCsv]
        public Guid? OrganisationId { get; set; }

        [DisplayName("Entiteit")]
        public string OrganisationName { get; set; }

        public FormalFrameworkBody(
            BodyDocument document,
            ApiConfigurationSection @params)
        {
            BodyId = document.Id;
            BodyName = document.Name;
            BodyNumber = document.BodyNumber;
            BodyShortName = document.ShortName;

            var organisation = document.Organisations.SingleOrDefault(x => x.IsBodyOrganisationActive());
            OrganisationId = organisation?.OrganisationId;
            OrganisationName = organisation?.Name;
        }

        public static async Task<IList<BodyDocument>> Search(
            IOpenSearchClient client,
            Guid formalFrameworkId,
            int scrollSize,
            string scrollTimeout)
        {
            var results = new List<BodyDocument>();

            var scroll = await client.SearchAsync<BodyDocument>(s => s
                .From(0)
                .Size(scrollSize)
                .Query(q => q
                    .Match(match => match
                        .Field(f => f.FormalFrameworks.Single().FormalFrameworkId)
                        .Query(formalFrameworkId.ToString())))
                .Source(source => source
                    .Includes(includes => includes
                        .Fields(
                            p => p.Id,
                            p => p.Name,
                            p => p.BodyNumber,
                            p => p.ShortName,
                            p => p.FormalFrameworks,
                            p => p.Organisations)))
                .Scroll(scrollTimeout));

            if (scroll.IsValid)
                results.AddRange(scroll.Documents);

            while (scroll.Documents.Any())
            {
                scroll = await client.ScrollAsync<BodyDocument>(scrollTimeout, scroll.ScrollId);

                if (scroll.IsValid)
                    results.AddRange(scroll.Documents);
            }

            return results;
        }

        public static IEnumerable<FormalFrameworkBody> Map(
            IEnumerable<BodyDocument> documents,
            Guid formalFrameworkId,
            ApiConfigurationSection @params)
        {
            var formalFrameworkBodies = new List<FormalFrameworkBody>();

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

                formalFrameworkBodies.Add(new FormalFrameworkBody(document, @params));
            }

            return formalFrameworkBodies;
        }

        public static IOrderedEnumerable<FormalFrameworkBody> Sort(
            IEnumerable<FormalFrameworkBody> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "organisationname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.OrganisationName)
                        : results.OrderByDescending(x => x.OrganisationName);
                case "bodyname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyName)
                        : results.OrderByDescending(x => x.BodyName);
                case "bodynumber":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyNumber)
                        : results.OrderByDescending(x => x.BodyNumber);
                default:
                    return results.OrderBy(x => x.BodyName);
            }
        }
    }

    public static class IsBodyOrganisationActiveExtension
    {
        public static bool IsBodyOrganisationActive(this BodyDocument.BodyOrganisation organisation)
        {
            return organisation.Validity == null ||
                   (!organisation.Validity.Start.HasValue || organisation.Validity.Start.Value <= DateTime.Now) &&
                   (!organisation.Validity.End.HasValue || organisation.Validity.End.Value >= DateTime.Now);
        }
    }
}
