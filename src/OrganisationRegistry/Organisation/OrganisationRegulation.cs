namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationRegulation : IOrganisationField, IValidityBuilder<OrganisationRegulation>
    {
        public Guid Id => OrganisationRegulationId;
        public Guid OrganisationId { get; }
        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationThemeId { get; }
        public string RegulationThemeName { get; }
        public string Link { get; }
        public Period Validity { get; }
        public DateTime? Date { get; }
        public string? Description { get; }

        public OrganisationRegulation(Guid organisationRegulationId,
            Guid organisationId,
            Guid? regulationThemeId,
            string regulationThemeName,
            string link,
            DateTime? date,
            string? description,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
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
                RegulationThemeId,
                RegulationThemeName,
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
