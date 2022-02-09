namespace OrganisationRegistry.Api.Backoffice.Report.Responses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using Nest;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SortOrder = Infrastructure.Search.Sorting.SortOrder;

    public class FormalFrameworkOrganisationVademecum
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

        [DisplayName("OVO-nummer")]
        public string OrganisationOvoNumber { get; set; }

        [DisplayName("Data.Vlaanderen link")]
        public Uri DataVlaanderenOrganisationUri { get; set; }

        [DisplayName("Vademecum sleutel")]
        public string VademecumKey { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="document"></param>
        /// <param name="params"></param>
        public FormalFrameworkOrganisationVademecum(
            OrganisationDocument document,
            ApiConfigurationSection @params)
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
            OrganisationOvoNumber = document.OvoNumber;

            DataVlaanderenOrganisationUri = new Uri(string.Format(@params.DataVlaanderenOrganisationUri, document.OvoNumber));

            VademecumKey = document.Keys
                ?.FirstOrDefault(x => x.KeyTypeId == @params.VademecumKeyTypeId && (x.Validity == null ||
                                      (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                                      (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now)))
                ?.Value;
        }

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
        /// Map each <see cref="OrganisationDocument"/> to a <see cref="FormalFrameworkOrganisation"/>
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="formalFrameworkId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkOrganisationVademecum> Map(
            IEnumerable<OrganisationDocument> documents,
            Guid formalFrameworkId,
            ApiConfigurationSection @params)
        {
            var formalFrameworkOrganisations = new List<FormalFrameworkOrganisationVademecum>();

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

                formalFrameworkOrganisations.Add(new FormalFrameworkOrganisationVademecum(document, @params));
            }

            return formalFrameworkOrganisations;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FormalFrameworkOrganisationVademecum> Sort(
            IEnumerable<FormalFrameworkOrganisationVademecum> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.OrganisationName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "vademecumkey":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.VademecumKey)
                        : results.OrderByDescending(x => x.VademecumKey);
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
