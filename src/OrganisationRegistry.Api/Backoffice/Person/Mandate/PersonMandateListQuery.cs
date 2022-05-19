namespace OrganisationRegistry.Api.Backoffice.Person.Mandate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using OrganisationRegistry.Api.Infrastructure.Search;
    using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using OrganisationRegistry.SqlServer.Infrastructure;
    using OrganisationRegistry.SqlServer.Person;

    public class PersonMandateListQueryResult
    {
        public Guid BodyId { get; }
        public string? BodyName { get; }

        public Guid? BodyOrganisationId { get; }
        public string? BodyOrganisationName { get; }

        public Guid BodySeatId { get; }
        public string? BodySeatName { get; }
        public string? BodySeatNumber { get; }

        public bool PaidSeat { get; set; }

        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public PersonMandateListQueryResult(
            Guid bodyId,
            string? bodyName,
            Guid? bodyOrganisationId,
            string? bodyOrganisationName,
            Guid bodySeatId,
            string? bodySeatName,
            string? bodySeatNumber,
            bool paidSeat,
            DateTime? validFrom, DateTime? validTo)
        {
            BodyId = bodyId;
            BodyName = bodyName;

            BodyOrganisationId = bodyOrganisationId;
            BodyOrganisationName = bodyOrganisationName;

            BodySeatId = bodySeatId;
            BodySeatName = bodySeatName;

            ValidFrom = validFrom;
            ValidTo = validTo;
            BodySeatNumber = bodySeatNumber;
            PaidSeat = paidSeat;
        }
    }

    public class PersonMandateListQuery : Query<PersonMandateListItem, PersonMandateListItem, PersonMandateListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _personId;

        protected override ISorting Sorting => new PersonMandateListSorting();

        protected override Expression<Func<PersonMandateListItem, PersonMandateListQueryResult>> Transformation =>
            x => new PersonMandateListQueryResult(
                x.BodyId,
                x.BodyName,
                x.BodyOrganisationId,
                x.BodyOrganisationName,
                x.BodySeatId,
                x.BodySeatName,
                x.BodySeatNumber,
                x.PaidSeat,
                x.ValidFrom,
                x.ValidTo);

        public PersonMandateListQuery(OrganisationRegistryContext context, Guid personId)
        {
            _context = context;
            _personId = personId;
        }

        protected override IQueryable<PersonMandateListItem> Filter(FilteringHeader<PersonMandateListItem> filtering)
            => _context.PersonMandateList
                .AsQueryable()
                .Where(x => x.PersonId == _personId).AsQueryable();

        private class PersonMandateListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(PersonMandateListItem.BodyName),
                nameof(PersonMandateListItem.BodySeatName),
                nameof(PersonMandateListItem.BodyOrganisationName),
                nameof(PersonMandateListItem.PaidSeat),
                nameof(PersonMandateListItem.ValidFrom),
                nameof(PersonMandateListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(PersonMandateListItem.BodyName), SortOrder.Ascending);
        }
    }
}
