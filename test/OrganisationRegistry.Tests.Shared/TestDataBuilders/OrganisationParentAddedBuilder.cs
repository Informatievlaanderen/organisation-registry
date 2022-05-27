namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation;
    using Organisation.Events;

    public class OrganisationParentAddedBuilder
    {
        public OrganisationId OrganisationId { get; }
        public Guid OrganisationOrganisationParentId { get; private set; }
        public OrganisationId ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public OrganisationParentAddedBuilder(Guid organisationId, Guid parentOrganisationId)
        {
            OrganisationOrganisationParentId = Guid.NewGuid();
            OrganisationId = new OrganisationId(organisationId);
            ParentOrganisationId = new OrganisationId(parentOrganisationId);
            ParentOrganisationName = OrganisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationParentAddedBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationParentAdded Build()
            => new(
                OrganisationId,
                OrganisationOrganisationParentId,
                ParentOrganisationId,
                ParentOrganisationName,
                ValidFrom,
                ValidTo);

        public static implicit operator OrganisationParentAdded(OrganisationParentAddedBuilder builder)
            => builder.Build();

        public OrganisationParentAddedBuilder WithOrganisationOrganisationParentId(
            Guid organisationOrganisationParentId)
        {
            OrganisationOrganisationParentId = organisationOrganisationParentId;
            return this;
        }
    }
}
