namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Domain;

    public interface IUniqueNameValidator<T> where T : AggregateRoot
    {
        bool IsNameTaken(string name);
        bool IsNameTaken(Guid id, string name);
    }
}
