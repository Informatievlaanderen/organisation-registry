namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Domain;

    public interface IUniqueNameWithinTypeValidator<T> where T : AggregateRoot
    {
        bool IsNameTaken(string name, Guid typeId);
        bool IsNameTaken(Guid id, string name, Guid typeId);
    }
}
