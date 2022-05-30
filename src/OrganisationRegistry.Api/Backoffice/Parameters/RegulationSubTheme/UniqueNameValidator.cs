namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme;

using System;
using System.Linq;
using OrganisationRegistry.RegulationSubTheme;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameWithinTypeValidator<RegulationSubTheme>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name, Guid typeId)
    {
        return
            _context.RegulationSubThemeList
                .AsQueryable()
                .Where(item => item.RegulationThemeId == typeId)
                .Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name, Guid typeId)
    {
        return
            _context.RegulationSubThemeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Where(item => item.RegulationThemeId == typeId)
                .Any(item => item.Name == name);
    }
}