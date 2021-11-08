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
        public Guid? RegulationSubThemeId { get; }
        public string RegulationSubThemeName { get; }
        public string Link { get; }
        public Period Validity { get; }
        public DateTime? Date { get; }
        public string? Description { get; }

        public OrganisationRegulation(Guid organisationRegulationId,
            Guid organisationId,
            Guid? regulationThemeId,
            string regulationThemeName,
            Guid? regulationSubThemeId,
            string regulationSubThemeName,
            string link,
            DateTime? date,
            string? description,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
            RegulationSubThemeId = regulationSubThemeId;
            RegulationSubThemeName = regulationSubThemeName;
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
                RegulationSubThemeId,
                RegulationSubThemeName,
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
