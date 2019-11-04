namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation.Events;

    public class OrganisationParentUpdatedTestDataBuilder
    {
        public Guid OrganisationId { get; }
        public Guid OrganisationOrganisationParentId { get; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public Guid PreviousParentOrganisationId { get; set; }
        public string PreviousParentOrganisationName { get; set; }

        public OrganisationParentUpdatedTestDataBuilder(Guid organisationOrganisationParentId, Guid organisationId, Guid parentOrganisationId)
        {
            OrganisationOrganisationParentId = organisationOrganisationParentId;
            OrganisationId = organisationId;
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationParentUpdatedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationParentUpdatedTestDataBuilder WithPreviousParent(Guid id)
        {
            PreviousParentOrganisationId = id;
            PreviousParentOrganisationName = id.ToString();
            return this;
        }

        public OrganisationParentUpdated Build()
            => new OrganisationParentUpdated(
                OrganisationId,
                OrganisationOrganisationParentId,
                ParentOrganisationId,
                ParentOrganisationName,
                ValidFrom,
                ValidTo,
                PreviousParentOrganisationId, PreviousParentOrganisationName, ValidFrom, ValidTo);
    }
}
