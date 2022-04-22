namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Organisation.Events;
    using Projections.Infrastructure;

    public class ScenarioBase<T>
    {
        protected readonly Fixture Fixture;

        public ScenarioBase(params ISpecimenBuilder[] specimenBuilders)
        {
            Fixture = new Fixture();

            Fixture.Register(() => new InitialiseProjection(typeof(T).FullName!));
            Fixture.Register<DateTime?>(() => Fixture.Create<DateTime>().Date);

            foreach (var specimenBuilder in specimenBuilders)
            {
                Fixture.Customizations.Add(specimenBuilder);
            }
        }

        public TU Create<TU>()
            => Fixture.Create<TU>();

        public void AddCustomization(ISpecimenBuilder customization)
            => Fixture.Customizations.Add(customization);

        public OrganisationTerminated CreateOrganisationTerminated(
            Guid organisationId,
            DateTime dateOfTermination,
            bool? forcedKboTermination = null,
            DateTime? dateOfTerminationAccordingToKbo = null,
            Dictionary<Guid, DateTime>? capacities = null)
            => new(
                organisationId,
                Create<string>(),
                Create<string>(),
                dateOfTermination,
                new FieldsToTerminate(
                    dateOfTermination,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    capacities ?? new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()
                ),
                new KboFieldsToTerminate(
                    new Dictionary<Guid, DateTime>(),
                    null,
                    null,
                    null
                ),
                forcedKboTermination ?? Create<bool>(),
                dateOfTerminationAccordingToKbo
            )
            {
                Version = 0,
                Timestamp = default
            };

        public OrganisationTerminatedV2 CreateOrganisationTerminatedV2(
            Guid organisationId,
            DateTime dateOfTermination,
            bool? forcedKboTermination = null,
            DateTime? dateOfTerminationAccordingToKbo = null,
            Dictionary<Guid, DateTime>? capacities = null)
            => new(
                organisationId,
                Create<string>(),
                Create<string>(),
                dateOfTermination,
                new FieldsToTerminateV2(
                    dateOfTermination,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    capacities ?? new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()
                ),
                new KboFieldsToTerminateV2(
                    new Dictionary<Guid, DateTime>(),
                    null,
                    null,
                    null
                ),
                forcedKboTermination ?? Create<bool>(),
                dateOfTerminationAccordingToKbo
            )
            {
                Version = 0,
                Timestamp = default
            };
    }
}
