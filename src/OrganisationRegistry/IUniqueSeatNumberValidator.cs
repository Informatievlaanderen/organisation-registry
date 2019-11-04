namespace OrganisationRegistry
{
    using System;

    public interface IUniqueSeatNumberValidator
    {
        bool IsBodyNumberTaken(string ovoNumber);
        bool IsBodyNumberTaken(Guid id, string ovoNumber);
    }
}
