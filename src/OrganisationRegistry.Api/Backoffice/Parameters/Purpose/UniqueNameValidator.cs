namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose;

using System;
using System.Linq;
using OrganisationRegistry.Purpose;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<Purpose>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.PurposeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.PurposeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}
