namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationRegulationAdded : BaseEvent<OrganisationRegulationAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationThemeId { get; }
        public string? RegulationThemeName { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationRegulationAdded(
            Guid organisationId,
            Guid organisationRegulationId,
            Guid? regulationThemeId,
            string? regulationThemeName,
            string? link,
            DateTime? date,
            string? description,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
            Link = link;
            Description = description;
            Date = date;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
