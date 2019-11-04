namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Organisation.Events;

    public class OrganisationFormalFrameworkAddedTestDataBuilder
    {
        public Guid OrganisationFormalFrameworkId { get; }
        public Guid OrganisationId { get; }
        public Guid FormalFrameworkId { get; }
        public string FormalFrameworkName { get; set; }
        public Guid ParentOrganisationId { get; }
        public string ParentOrganisationName { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public OrganisationFormalFrameworkAddedTestDataBuilder(Guid organisationId, Guid formalFrameworkId, Guid parentOrganisationId)
        {
            OrganisationFormalFrameworkId = Guid.NewGuid();
            OrganisationId = organisationId;
            FormalFrameworkId = formalFrameworkId;
            FormalFrameworkName = parentOrganisationId.ToString();
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationName = parentOrganisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationFormalFrameworkAddedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationFormalFrameworkAdded Build()
            => new OrganisationFormalFrameworkAdded(
                OrganisationId,
                OrganisationFormalFrameworkId,
                FormalFrameworkId, FormalFrameworkName,
                ParentOrganisationId, ParentOrganisationName,
                ValidFrom,
                ValidTo);
    }
}
