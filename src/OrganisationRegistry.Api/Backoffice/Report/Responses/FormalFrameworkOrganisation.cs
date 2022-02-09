namespace OrganisationRegistry.Api.Backoffice.Report.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using Infrastructure.Search.Sorting;
    using Nest;
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
        /// <param name="today"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkOrganisationBase> Map(IEnumerable<OrganisationDocument> documents,
            Guid formalFrameworkId,
            ApiConfigurationSection @params,
            DateTime today)
        {
            var formalFrameworkOrganisations = new List<FormalFrameworkOrganisationBase>();

            foreach (var document in documents)
            {
                var formalFrameworks = document
                    .FormalFrameworks?
                    .Where(x =>
                        x.FormalFrameworkId == formalFrameworkId &&
                        (x.Validity == null ||
                         x.Validity.OverlapsWith(today)))
                    .ToList();

                if (formalFrameworks == null || !formalFrameworks.Any())
                    continue;

                formalFrameworkOrganisations.Add(
                    new FormalFrameworkOrganisationBase(
                        document,
                        @params,
                        formalFrameworkId,
                        today));
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
            ApiConfigurationSection @params,
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
                         x.Validity.OverlapsWith(today)))
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
}
