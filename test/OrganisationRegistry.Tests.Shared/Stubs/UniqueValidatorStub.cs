namespace OrganisationRegistry.Tests.Shared.Stubs;

using System;
using Infrastructure.Domain;

public class UniqueValidatorStub<T> : IUniqueNameWithinTypeValidator<T> where T : AggregateRoot
{
    private readonly bool _isUnique;

    public UniqueValidatorStub(bool isUnique = true)
    {
        _isUnique = isUnique;
    }

    public bool IsNameTaken(string name, Guid typeId)
        => !_isUnique;

    public bool IsNameTaken(Guid id, string name, Guid typeId)
        => !_isUnique;
}
