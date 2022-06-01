namespace OrganisationRegistry.Api.Backoffice.Parameters.Building;

using System;
using System.Linq;
using OrganisationRegistry.Building;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<Building>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.BuildingList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.BuildingList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}
