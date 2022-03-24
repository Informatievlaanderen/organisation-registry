namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Organisation.OrganisationTermination;

    public class OrganisationTerminationSummaryBuilder
    {
        private OrganisationTerminationSummary _organisationTerminationSummary = new(
            null,
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>(),
            new Dictionary<Guid, DateTime>());

        public OrganisationTerminationSummaryBuilder(ISpecimenBuilder fixture)
        {
            _organisationTerminationSummary = fixture.Create<OrganisationTerminationSummary>();
        }

        public OrganisationTerminationSummaryBuilder()
        {
        }

        public OrganisationTerminationSummaryBuilder WithOrganisationNewValidTo(DateTime? value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { OrganisationNewValidTo = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithContacts(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Contacts = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithBankAccounts(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { BankAccounts = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithFunctions(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Functions = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithLocations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Locations = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithCapacities(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Capacities = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithBuildings(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Buildings = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithLabels(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Labels = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithRelations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Relations = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithOpeningHours(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { OpeningHours = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithClassifications(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Classifications = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithFormalFrameworks(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { FormalFrameworks = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithRegulations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Regulations = value };
            return this;
        }

        public OrganisationTerminationSummaryBuilder WithKeys(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Keys = value };
            return this;
        }

        public OrganisationTerminationSummary Build()
            => _organisationTerminationSummary;

        public static implicit operator OrganisationTerminationSummary(OrganisationTerminationSummaryBuilder dataBuilder)
            => dataBuilder.Build();
    }
}
