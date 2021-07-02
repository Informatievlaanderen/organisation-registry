namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationRegulation : IOrganisationField, IValidityBuilder<OrganisationRegulation>
    {
        public Guid Id => OrganisationRegulationId;
        public Guid OrganisationId { get; }
        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationTypeId { get; }
        public string RegulationTypeName { get; }
        public string Link { get; }
        public Period Validity { get; }
        public DateTime? Date { get; }
        public string? Description { get; }

        public OrganisationRegulation(Guid organisationRegulationId,
            Guid organisationId,
            Guid? regulationTypeId,
            string regulationTypeName,
            string link,
            DateTime? date,
            string? description,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationRegulationId = organisationRegulationId;
            RegulationTypeId = regulationTypeId;
            RegulationTypeName = regulationTypeName;
            Link = link;
            Validity = validity;
            Date = date;
            Description = description;
        }

        public OrganisationRegulation WithValidity(Period period)
        {
            return new OrganisationRegulation(
                OrganisationRegulationId,
                OrganisationId,
                RegulationTypeId,
                RegulationTypeName,
                Link,
                Date,
                Description,
                period);
        }

        public OrganisationRegulation WithValidFrom(ValidFrom validFrom)
        {
            return WithValidity(new Period(validFrom, Validity.End));
        }

        public OrganisationRegulation WithValidTo(ValidTo validTo)
        {
            return WithValidity(new Period(Validity.Start, validTo));
        }
    }
}
