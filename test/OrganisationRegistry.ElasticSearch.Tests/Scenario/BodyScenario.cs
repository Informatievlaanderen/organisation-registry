namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Projections.Body;
    using Projections.Infrastructure;
    using Specimen;

    /// <summary>
    /// Sets up a fixture which uses the same bodyId and LifecyclePhaseTypeId for all events
    /// </summary>
    public class BodyScenarioBase : ScenarioBase
    {
        public BodyScenarioBase(Guid bodyId) :
            base(
                new ParameterNameArg("bodyId", bodyId),
                new ParameterNameArg("lifecyclePhaseTypeId", Guid.NewGuid()),
                new ParameterNameArg("bodySeatId", Guid.NewGuid()),
                new ParameterNameArg("bodyMandateId", Guid.NewGuid()),
                new ParameterNameArg("delegationAssignmentId", Guid.NewGuid()),
                new ParameterNameArg("personId", Guid.NewGuid()),
                new ParameterNameArg("organisationId", Guid.NewGuid()))
        {
            var functionTypeId = Guid.NewGuid();
            AddCustomization(new ParameterNameArg("functionId", functionTypeId));
            AddCustomization(new ParameterNameArg("functionTypeId", functionTypeId));
        }
    }

    public class ScenarioBase
    {
        private readonly Fixture _fixture;

        public ScenarioBase(params ISpecimenBuilder[] specimenBuilders)
        {
            _fixture = new Fixture();

            _fixture.Register(() => new InitialiseProjection(typeof(BodyHandler).FullName));
            _fixture.Register<DateTime?>(() => null);

            foreach (var specimenBuilder in specimenBuilders)
            {
                _fixture.Customizations.Add(specimenBuilder);
            }
        }

        public T Create<T>()
        {
            return _fixture.Create<T>();
        }

        public void AddCustomization(ISpecimenBuilder customization)
        {
            _fixture.Customizations.Add(customization);
        }
    }
}
