namespace OrganisationRegistry;

using System;
using Infrastructure.Domain;

// ReSharper disable once UnusedTypeParameter
public interface IUniqueNameWithinTypeValidator<T> where T : AggregateRoot
{
    bool IsNameTaken(string name, Guid typeId);
    bool IsNameTaken(Guid id, string name, Guid typeId);
}
