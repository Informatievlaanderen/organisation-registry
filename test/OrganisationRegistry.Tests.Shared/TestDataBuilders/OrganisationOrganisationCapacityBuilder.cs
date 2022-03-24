namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Organisation;

    public class OrganisationOrganisationCapacityBuilder
    {
        private OrganisationCapacity _organisationOrganisationCapacity;

        public OrganisationOrganisationCapacityBuilder(ISpecimenBuilder fixture)
        {
            _organisationOrganisationCapacity = new OrganisationCapacity(
                fixture.Create<Guid>(),
                fixture.Create<Guid>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Guid>(),
                fixture.Create<string>(),
                fixture.Create<Dictionary<Guid, string>>(),
                fixture.Create<Period>(),
                fixture.Create<bool>()
            );
        }
        public OrganisationOrganisationCapacityBuilder WithValidity(ValidFrom from, ValidTo to)
        {
            _organisationOrganisationCapacity = _organisationOrganisationCapacity.WithValidity(new Period(from, to));
            return this;
        }

        public OrganisationOrganisationCapacityBuilder WithOrganisationCapacityId(Guid capacityId)
        {
            _organisationOrganisationCapacity = new OrganisationCapacity(
                _organisationOrganisationCapacity.OrganisationCapacityId,
                _organisationOrganisationCapacity.OrganisationId,
                capacityId,
                _organisationOrganisationCapacity.CapacityName,
                _organisationOrganisationCapacity.PersonId,
                _organisationOrganisationCapacity.PersonName,
                _organisationOrganisationCapacity.FunctionTypeId,
                _organisationOrganisationCapacity.FunctionTypeName,
                _organisationOrganisationCapacity.LocationId,
                _organisationOrganisationCapacity.LocationName,
                _organisationOrganisationCapacity.Contacts,
                _organisationOrganisationCapacity.Validity,
                _organisationOrganisationCapacity.IsActive);
            return this;
        }

        public OrganisationCapacity Build()
            => _organisationOrganisationCapacity;

        public static implicit operator OrganisationCapacity(OrganisationOrganisationCapacityBuilder builder)
            => builder.Build();
    }
}
