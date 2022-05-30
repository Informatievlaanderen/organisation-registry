namespace OrganisationRegistry.Infrastructure.Domain.Exception;

using System;

public class MissingParameterLessConstructor : Exception
{
    public MissingParameterLessConstructor(Type type)
        : base($"{type.FullName} has no constructor without paramerters. This can be either public or private") { }
}