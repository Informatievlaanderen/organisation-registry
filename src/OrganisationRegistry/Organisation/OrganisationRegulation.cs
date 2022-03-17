namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationRegulation : IOrganisationField, IValidityBuilder<OrganisationRegulation>
    {
        public Guid Id => OrganisationRegulationId;
        public Guid OrganisationId { get; }
        public Guid OrganisationRegulationId { get; }
        public Guid? RegulationThemeId { get; }
        public string? RegulationThemeName { get; }
        public Guid? RegulationSubThemeId { get; }
        public string? RegulationSubThemeName { get; }
        public string Name { get; }
        public string? Link { get; }
        public string? WorkRulesUrl { get; }
        public Period Validity { get; }
        public DateTime? Date { get; }
        public string? Description { get; }
        public string? DescriptionRendered { get; }

        public OrganisationRegulation(Guid organisationRegulationId,
            Guid organisationId,
            Guid? regulationThemeId,
            string? regulationThemeName,
            Guid? regulationSubThemeId,
            string? regulationSubThemeName,
            string name,
            string? link,
            string? workRulesUrl,
            DateTime? date,
            string? description,
            string? descriptionRendered,
            Period validity)
        {
            OrganisationId = organisationId;
            OrganisationRegulationId = organisationRegulationId;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;
            RegulationSubThemeId = regulationSubThemeId;
            RegulationSubThemeName = regulationSubThemeName;
            Name = name;
            Link = link;
            WorkRulesUrl = workRulesUrl;
            Validity = validity;
            Date = date;
            Description = description;
            DescriptionRendered = descriptionRendered;
        }

        public OrganisationRegulation WithValidity(Period period)
            => new(
                OrganisationRegulationId,
                OrganisationId,
                RegulationThemeId,
                RegulationThemeName,
                RegulationSubThemeId,
                RegulationSubThemeName,
                Name,
                Link,
                WorkRulesUrl,
                Date,
                Description,
                DescriptionRendered,
                period);

        public OrganisationRegulation WithValidFrom(ValidFrom validFrom)
            => WithValidity(new Period(validFrom, Validity.End));

        public OrganisationRegulation WithValidTo(ValidTo validTo)
            => WithValidity(new Period(Validity.Start, validTo));
    }
}
