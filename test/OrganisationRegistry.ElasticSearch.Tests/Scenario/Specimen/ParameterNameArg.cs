using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace OrganisationRegistry.ElasticSearch.Tests.Scenario.Specimen
{
    public class ParameterNameArg : ISpecimenBuilder
    {
        private readonly string _parameterName;
        private readonly Guid _guid;

        public ParameterNameArg(string parameterName, Guid guid)
        {
            _parameterName = parameterName;
            _guid = guid;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (!(request is ParameterInfo pi))
                return new NoSpecimen();

            if (pi.ParameterType != typeof(Guid) ||
                pi.Name != _parameterName)
                return new NoSpecimen();

            return _guid;
        }
    }
}
