namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using AutoFixture;
    using AutoFixture.Kernel;
    using Projections.Infrastructure;

    public class ScenarioBase<T>
    {
        private readonly Fixture _fixture;

        public ScenarioBase(params ISpecimenBuilder[] specimenBuilders)
        {
            _fixture = new Fixture();

            _fixture.Register(() => new InitialiseProjection(typeof(T).FullName));
            _fixture.Register<DateTime?>(() => _fixture.Create<DateTime>().Date);

            foreach (var specimenBuilder in specimenBuilders)
            {
                _fixture.Customizations.Add(specimenBuilder);
            }
        }

        public TU Create<TU>()
        {
            return _fixture.Create<TU>();
        }

        public void AddCustomization(ISpecimenBuilder customization)
        {
            _fixture.Customizations.Add(customization);
        }
    }
}
