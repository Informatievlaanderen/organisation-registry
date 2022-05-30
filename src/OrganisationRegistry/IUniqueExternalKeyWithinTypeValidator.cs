namespace OrganisationRegistry;

using System;
using Infrastructure.Domain;

// ReSharper disable once UnusedTypeParameter
public interface IUniqueExternalKeyWithinTypeValidator<T> where T : AggregateRoot
{
    bool IsExternalKeyTaken(string? externalKey, Guid typeId);
    bool IsExternalKeyTaken(Guid id, string? externalKey, Guid typeId);
}