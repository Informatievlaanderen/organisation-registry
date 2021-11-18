namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using RegulationSubTheme;
    using RegulationTheme;

    public class AddOrganisationRegulation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public RegulationThemeId RegulationThemeId { get; }
        public RegulationSubThemeId RegulationSubThemeId { get; }
        public string RegulationName { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public string? DescriptionRendered { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationRegulation(Guid organisationRegulationId,
            OrganisationId organisationId,
            RegulationThemeId regulationThemeId,
            RegulationSubThemeId regulationSubThemeId,
            string regulationName,
            string? link,
            DateTime? date,
            string? description,
            string? descriptionRendered,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationSubThemeId = regulationSubThemeId;
            RegulationName = regulationName;
            Link = link;
            Date = date;
            Description = description;
            DescriptionRendered = descriptionRendered;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
