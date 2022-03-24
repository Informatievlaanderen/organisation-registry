namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Organisation;

    public class OrganisationOrganisationClassificationTestDataBuilder
    {
        private OrganisationOrganisationClassification _organisationOrganisationClassification;

        public OrganisationOrganisationClassificationTestDataBuilder(ISpecimenBuilder fixture)
        {
            _organisationOrganisationClassification = new OrganisationOrganisationClassification(
                fixture.Create<Guid>(),
                fixture.Create<Guid>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Period>()
            );
        }
        public OrganisationOrganisationClassificationTestDataBuilder WithValidity(ValidFrom from, ValidTo to)
        {
            _organisationOrganisationClassification = _organisationOrganisationClassification.WithValidity(new Period(from, to));
            return this;
        }

        public OrganisationOrganisationClassificationTestDataBuilder WithOrganisationClassificationId(Guid organisationClassificationTypeId)
        {
            _organisationOrganisationClassification = new OrganisationOrganisationClassification(
                _organisationOrganisationClassification.OrganisationOrganisationClassificationId,
                _organisationOrganisationClassification.OrganisationId,
                organisationClassificationTypeId,
                _organisationOrganisationClassification.OrganisationClassificationTypeName,
                _organisationOrganisationClassification.OrganisationClassificationId,
                _organisationOrganisationClassification.OrganisationClassificationName,
                _organisationOrganisationClassification.Validity);
            return this;
        }

        public OrganisationOrganisationClassification Build()
            => _organisationOrganisationClassification;

        public static implicit operator OrganisationOrganisationClassification(OrganisationOrganisationClassificationTestDataBuilder builder)
            => builder.Build();
    }
}
