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
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationRegulation(Guid organisationRegulationId,
            OrganisationId organisationId,
            RegulationThemeId regulationThemeId,
            RegulationSubThemeId regulationSubThemeId,
            string? link,
            DateTime? date,
            string? description,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationSubThemeId = regulationSubThemeId;
            Link = link;
            Date = date;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
