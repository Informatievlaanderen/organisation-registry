namespace OrganisationRegistry;

using System;

public interface IUniqueBodyNumberValidator
{
    bool IsBodyNumberTaken(string ovoNumber);
    bool IsBodyNumberTaken(Guid id, string ovoNumber);
}