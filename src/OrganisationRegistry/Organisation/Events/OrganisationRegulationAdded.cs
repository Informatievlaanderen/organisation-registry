namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationRegulationAdded : BaseEvent<OrganisationRegulationAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationThemeId { get; }
        public string? RegulationThemeName { get; }
        public Guid? RegulationSubThemeId { get; }
        public string? RegulationSubThemeName { get; }
        public string Name { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public string? DescriptionRendered { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public OrganisationRegulationAdded(Guid organisationId,
            Guid organisationRegulationId,
            Guid? regulationThemeId,
            string? regulationThemeName,
            Guid? regulationSubThemeId,
            string? regulationSubThemeName,
            string name,
            string? link,
            DateTime? date,
            string? description,
            string? descriptionRendered,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
            RegulationSubThemeId = regulationSubThemeId;
            RegulationSubThemeName = regulationSubThemeName;
            Name = name;
            Link = link;
            Description = description;
            DescriptionRendered = descriptionRendered;
            Date = date;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
