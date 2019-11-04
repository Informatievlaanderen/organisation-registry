namespace OrganisationRegistry.Infrastructure.Domain.Exception
{
    using System;

    public class MissingParameterLessConstructorException : Exception
    {
        public MissingParameterLessConstructorException(Type type)
            : base($"{type.FullName} has no constructor without paramerters. This can be either public or private") { }
    }
}
