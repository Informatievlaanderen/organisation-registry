namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Domain;

    // ReSharper disable once UnusedTypeParameter
    public interface IUniqueCodeValidator<T> where T : AggregateRoot
    {
        bool IsCodeTaken(string name);
        bool IsCodeTaken(Guid id, string name);
    }
}
