namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationRegulationUpdated : BaseEvent<OrganisationRegulationUpdated>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }

        public Guid? RegulationThemeId { get; }
        public Guid? PreviousRegulationThemeId { get; }

        public string? RegulationThemeName { get; }
        public string? PreviousRegulationThemeName { get; }

        public Guid? RegulationSubThemeId { get; }
        public Guid? PreviousRegulationSubThemeId { get; }

        public string? RegulationSubThemeName { get; }
        public string? PreviousRegulationSubThemeName { get; }

        public string? Link { get; }
        public string? PreviousLink { get; }

        public DateTime? Date { get; }
        public DateTime? PreviousDate { get; }

        public string? Description { get; }
        public string? PreviousDescription { get; }

        public DateTime? ValidFrom { get; }
        public DateTime? PreviouslyValidFrom { get; }

        public DateTime? ValidTo { get; }

        public DateTime? PreviouslyValidTo { get; }

        public OrganisationRegulationUpdated(
            Guid organisationId,
            Guid organisationRegulationId,
            Guid? regulationThemeId,
            string regulationThemeName,
            Guid? regulationSubThemeId,
            string regulationSubThemeName,
            string? link,
            DateTime? date,
            string? description,
            DateTime? validFrom,
            DateTime? validTo,
            Guid? previousRegulationThemeId,
            string previousRegulationThemeName,
            Guid? previousRegulationSubThemeId,
            string previousRegulationSubThemeName,
            DateTime? previouslyValidFrom,
            DateTime? previouslyValidTo,
            string? previousLink,
            DateTime? previousDate,
            string? previousDescription)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
            RegulationSubThemeId = regulationSubThemeId;
            RegulationSubThemeName = regulationSubThemeName;
            Link = link;
            Date = date;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;

            PreviousRegulationThemeId = previousRegulationThemeId;
            PreviousRegulationThemeName = previousRegulationThemeName;
            PreviousRegulationSubThemeId = previousRegulationSubThemeId;
            PreviousRegulationSubThemeName = previousRegulationSubThemeName;
            PreviousLink = previousLink;
            PreviousDate = previousDate;
            PreviousDescription = previousDescription;
            PreviouslyValidFrom = previouslyValidFrom;
            PreviouslyValidTo = previouslyValidTo;
        }
    }
}
