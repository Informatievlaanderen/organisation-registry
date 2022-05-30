namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme;

using System;
using System.Linq;
using OrganisationRegistry.RegulationTheme;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<RegulationTheme>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.RegulationThemeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.RegulationThemeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}