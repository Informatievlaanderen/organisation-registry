namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Domain;

    public interface IUniqueExternalKeyWithinTypeValidator<T> where T : AggregateRoot
    {
        bool IsExternalKeyTaken(string externalKey, Guid typeId);
        bool IsExternalKeyTaken(Guid id, string externalKey, Guid typeId);
    }
}
