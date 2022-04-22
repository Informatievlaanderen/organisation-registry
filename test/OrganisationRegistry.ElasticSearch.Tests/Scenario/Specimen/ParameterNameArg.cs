using System.Reflection;
using AutoFixture.Kernel;

namespace OrganisationRegistry.ElasticSearch.Tests.Scenario.Specimen
{
    using System;

    public class ParameterNameArg<T> : ISpecimenBuilder
    {
        private readonly string _parameterName;
        private readonly T _value;

        public ParameterNameArg(string parameterName, T value)
        {
            _parameterName = parameterName;
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            Console.WriteLine("Create Specimen");

            if (!(request is ParameterInfo pi))
                return new NoSpecimen();

            if (pi.ParameterType != typeof(T) ||
                pi.Name != _parameterName)
                return new NoSpecimen();

            return _value ?? (object)new NoSpecimen();
        }
    }
}
