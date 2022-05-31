namespace OrganisationRegistry;

using System;

public interface IUniqueOvoNumberValidator
{
    bool IsOvoNumberTaken(string? ovoNumber);
    bool IsOvoNumberTaken(Guid id, string ovoNumber);
}
