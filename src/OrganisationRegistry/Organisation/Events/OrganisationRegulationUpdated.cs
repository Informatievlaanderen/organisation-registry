namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationRegulationUpdated : BaseEvent<OrganisationRegulationUpdated>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationRegulationId { get; }

        public Guid? RegulationTypeId { get; }
        public Guid? PreviousRegulationTypeId { get; }

        public string? RegulationTypeName { get; }
        public string? PreviousRegulationTypeName { get; }

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
            Guid? regulationTypeId,
            string regulationTypeName,
            string? link,
            DateTime? date,
            string? description,
            DateTime? validFrom,
            DateTime? validTo,
            Guid? previousRegulationTypeId,
            string previousRegulationTypeName,
            DateTime? previouslyValidFrom,
            DateTime? previouslyValidTo,
            string? previousLink,
            DateTime? previousDate,
            string? previousDescription)
        {
            Id = organisationId;

            OrganisationRegulationId = organisationRegulationId;
            RegulationTypeId = regulationTypeId;
            RegulationTypeName = regulationTypeName;
            Link = link;
            Date = date;
            Description = description;
            ValidFrom = validFrom;
            ValidTo = validTo;

            PreviousRegulationTypeId = previousRegulationTypeId;
            PreviousRegulationTypeName = previousRegulationTypeName;
            PreviousLink = previousLink;
            PreviousDate = previousDate;
            PreviousDescription = previousDescription;
            PreviouslyValidFrom = previouslyValidFrom;
            PreviouslyValidTo = previouslyValidTo;
        }
    }
}
