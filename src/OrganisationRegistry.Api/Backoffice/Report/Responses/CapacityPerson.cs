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

    public class CapacityPerson
    {
        [ExcludeFromCsv]
        public Guid? ParentOrganisationId { get; set; }

        [DisplayName("Moeder entiteit")]
        public string ParentOrganisationName { get; set; }

        [ExcludeFromCsv]
        public Guid OrganisationId { get; set; }

        [DisplayName("Entiteit")]
        public string OrganisationName { get; set; }

        [DisplayName("Entiteit ovo-nummer")]
        public string OvoNumber { get; set; }

        [DisplayName("Korte naam")]
        public string OrganisationShortName { get; set; }

        [ExcludeFromCsv]
        public Guid? PersonId { get; set; }

        [DisplayName("Persoon")]
        public string PersonName { get; set; }

        [ExcludeFromCsv]
        public Guid? FunctionTypeId { get; set; }

        [DisplayName("Functie")]
        public string FunctionTypeName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Locatie")]
        public string Location { get; set; }

        [DisplayName("Telefoon")]
        public string Phone { get; set; }

        [DisplayName("Gsm")]
        public string CellPhone { get; set; }

        [DisplayName("Beleidsdomein")]
        public string PolicyDomain { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="document"></param>
        /// <param name="capacity"></param>
        /// <param name="params"></param>
        public CapacityPerson(
            OrganisationDocument document,
            OrganisationDocument.OrganisationCapacity capacity,
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
            OvoNumber = document.OvoNumber;
            OrganisationShortName = document.ShortName;

            PersonId = capacity.PersonId;
            PersonName = capacity.PersonName;
            FunctionTypeId = capacity.FunctionId;
            FunctionTypeName = capacity.FunctionName;

            Location = document
                .Locations?
                .FirstOrDefault(x =>
                    x.IsMainLocation && (x.Validity == null ||
                                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                                         (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now)))?
                .FormattedAddress;

            Email = capacity.Contacts?.FirstOrDefault(x => x.ContactTypeId == @params.EmailContactTypeId)?.Value;
            Phone = capacity.Contacts?.FirstOrDefault(x => x.ContactTypeId == @params.PhoneContactTypeId)?.Value;
            CellPhone = capacity.Contacts?.FirstOrDefault(x => x.ContactTypeId == @params.CellPhoneContactTypeId)?.Value;

            PolicyDomain = document.OrganisationClassifications
                ?.FirstOrDefault(x => x.OrganisationClassificationTypeId == @params.PolicyDomainClassificationTypeId)
                ?.OrganisationClassificationName;
        }

        /// <summary>
        /// Scroll through all <see cref="OrganisationDocument"/> with a matching (and active) <see cref="Capacity"/>, and return entire dataset
        /// </summary>
        /// <param name="client"></param>
        /// <param name="capacityId"></param>
        /// <param name="scrollSize"></param>
        /// <param name="scrollTimeout"></param>
        /// <returns></returns>
        public static async Task<IList<OrganisationDocument>> Search(
            IElasticClient client,
            Guid capacityId,
            int scrollSize,
            string scrollTimeout)
        {
            var results = new List<OrganisationDocument>();

            var scroll = await client.SearchAsync<OrganisationDocument>(s => s
                .From(0)
                .Size(scrollSize)
                .Query(q => q
                    .Match(match => match
                        .Field(f => f.Capacities.Single().CapacityId)
                        .Query(capacityId.ToString())))
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
        /// Map each <see cref="OrganisationDocument"/> to a <see cref="CapacityPerson"/>
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="capacityId"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static IEnumerable<CapacityPerson> Map(
            IEnumerable<OrganisationDocument> documents,
            Guid capacityId,
            ApiConfiguration @params)
        {
            var capacityPersons = new List<CapacityPerson>();

            foreach (var document in documents)
            {
                var capacities = document
                    .Capacities?
                    .Where(x =>
                        x.CapacityId == capacityId &&
                        (x.Validity == null ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now)))
                    .ToList();

                if (capacities == null || !capacities.Any())
                    continue;

                foreach (var capacity in capacities)
                {
                    if (!capacity.PersonId.HasValue)
                        continue;

                    capacityPersons.Add(new CapacityPerson(document, capacity, @params));
                }
            }

            return capacityPersons;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<CapacityPerson> Sort(
            IEnumerable<CapacityPerson> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.OrganisationName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "personname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.PersonName)
                        : results.OrderByDescending(x => x.PersonName);
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
