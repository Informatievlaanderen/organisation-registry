namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using RegulationType;

    public class AddOrganisationRegulation : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }
        public RegulationTypeId RegulationTypeId { get; }
        public string? Link { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationRegulation(Guid organisationRegulationId,
            OrganisationId organisationId,
            RegulationTypeId regulationTypeId,
            string? link,
            DateTime? date,
            string? description,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationTypeId = regulationTypeId;
            Link = link;
            Date = date;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
