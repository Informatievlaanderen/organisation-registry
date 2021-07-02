namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationRegulationAdded : BaseEvent<OrganisationRegulationAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationTypeId { get; }
        public string? RegulationTypeName { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationRegulationAdded(
            Guid organisationId,
            Guid organisationRegulationId,
            Guid? regulationTypeId,
            string? regulationTypeName,
            string? link,
            DateTime? date,
            string? description,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationTypeId = regulationTypeId;
            RegulationTypeName = regulationTypeName;
            Link = link;
            Description = description;
            Date = date;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
