namespace OrganisationRegistry.Api.Backoffice.Body;

using System;
using System.Linq;
using SqlServer.Infrastructure;

public class UniqueSeatNumberValidator : IUniqueSeatNumberValidator
{
    private readonly OrganisationRegistryContext _context;

    public UniqueSeatNumberValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsBodyNumberTaken(string bodyNumber)
    {
        return _context.BodyDetail.Any(item => item.BodyNumber == bodyNumber);
    }

    public bool IsBodyNumberTaken(Guid id, string bodyNumber)
    {
        return _context.BodyDetail
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.BodyNumber == bodyNumber);
    }
}