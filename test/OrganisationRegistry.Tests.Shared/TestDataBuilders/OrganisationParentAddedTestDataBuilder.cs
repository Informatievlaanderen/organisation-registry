namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation;
    using Organisation.Events;

    public class OrganisationParentAddedTestDataBuilder
    {
        public OrganisationId OrganisationId { get; }
        public Guid OrganisationOrganisationParentId { get; }
        public OrganisationId ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public OrganisationParentAddedTestDataBuilder(Guid organisationId, Guid parentOrganisationId)
        {
            OrganisationOrganisationParentId = Guid.NewGuid();
            OrganisationId = new OrganisationId(organisationId);
            ParentOrganisationId = new OrganisationId(parentOrganisationId);
            ParentOrganisationName = OrganisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationParentAddedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationParentAdded Build()
            => new OrganisationParentAdded(
                OrganisationId,
                OrganisationOrganisationParentId,
                ParentOrganisationId,
                ParentOrganisationName,
                ValidFrom,
                ValidTo);
    }
}
