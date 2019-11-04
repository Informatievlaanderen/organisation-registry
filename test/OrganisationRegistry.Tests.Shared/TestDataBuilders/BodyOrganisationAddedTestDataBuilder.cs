namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Body.Events;

    public class BodyOrganisationAddedTestDataBuilder
    {
        public Guid BodyId { get; set; }
        public Guid BodyOrganisationId { get; set; }
        public string BodyName { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public BodyOrganisationAddedTestDataBuilder(Guid bodyId, Guid organisationId)
        {
            BodyOrganisationId = Guid.NewGuid();
            BodyId = bodyId;
            BodyName = bodyId.ToString();
            OrganisationId = organisationId;
            OrganisationName = organisationId.ToString();
            ValidFrom = null;
            ValidTo = null;
        }

        public BodyOrganisationAdded Build()
            => new BodyOrganisationAdded(
                BodyId,
                BodyOrganisationId,
                BodyName,
                OrganisationId,
                OrganisationName,
                ValidFrom,
                ValidTo);
    }
}
