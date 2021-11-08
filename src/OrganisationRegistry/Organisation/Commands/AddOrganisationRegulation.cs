namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using RegulationTheme;

    public class AddOrganisationRegulation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public RegulationThemeId RegulationThemeId { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationRegulation(Guid organisationRegulationId,
            OrganisationId organisationId,
            RegulationThemeId regulationThemeId,
            string? link,
            DateTime? date,
            string? description,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            Link = link;
            Date = date;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
