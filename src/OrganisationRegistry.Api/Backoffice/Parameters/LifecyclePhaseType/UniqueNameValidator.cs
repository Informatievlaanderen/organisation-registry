namespace OrganisationRegistry.Api.Backoffice.Parameters.LifecyclePhaseType;

using System;
using System.Linq;
using OrganisationRegistry.LifecyclePhaseType;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<LifecyclePhaseType>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.LifecyclePhaseTypeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.LifecyclePhaseTypeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}
