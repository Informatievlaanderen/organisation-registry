namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework;

using System;
using System.Linq;
using OrganisationRegistry.FormalFramework;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameWithinTypeValidator<FormalFramework>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name, Guid categoryId)
    {
        return
            _context.FormalFrameworkList
                .AsQueryable()
                .Where(item => item.FormalFrameworkCategoryId == categoryId)
                .Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name, Guid categoryId)
    {
        return _context.FormalFrameworkList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Where(item => item.FormalFrameworkCategoryId == categoryId)
            .Any(item => item.Name == name);
    }
}
