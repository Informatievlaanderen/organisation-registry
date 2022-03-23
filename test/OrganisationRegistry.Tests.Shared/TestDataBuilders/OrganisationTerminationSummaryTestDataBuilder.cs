namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Organisation.OrganisationTermination;

    public class OrganisationTerminationSummaryTestDataBuilder
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

        public OrganisationTerminationSummaryTestDataBuilder(Fixture fixture)
        {
            _organisationTerminationSummary = fixture.Create<OrganisationTerminationSummary>();
        }

        public OrganisationTerminationSummaryTestDataBuilder()
        {
        }

        public OrganisationTerminationSummaryTestDataBuilder WithOrganisationNewValidTo(DateTime? value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { OrganisationNewValidTo = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithContacts(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Contacts = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithBankAccounts(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { BankAccounts = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithFunctions(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Functions = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithLocations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Locations = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithCapacities(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Capacities = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithBuildings(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Buildings = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithLabels(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Labels = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithRelations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Relations = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithOpeningHours(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { OpeningHours = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithClassifications(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Classifications = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithFormalFrameworks(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { FormalFrameworks = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithRegulations(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Regulations = value };
            return this;
        }

        public OrganisationTerminationSummaryTestDataBuilder WithKeys(Dictionary<Guid, DateTime> value)
        {
            _organisationTerminationSummary = _organisationTerminationSummary with { Keys = value };
            return this;
        }

        public OrganisationTerminationSummary Build()
            => _organisationTerminationSummary;

        public static implicit operator OrganisationTerminationSummary(OrganisationTerminationSummaryTestDataBuilder dataBuilder)
            => dataBuilder.Build();
    }
}
