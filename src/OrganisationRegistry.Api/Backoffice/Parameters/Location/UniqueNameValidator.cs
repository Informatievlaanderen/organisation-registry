namespace OrganisationRegistry.Api.Backoffice.Parameters.Location;

using System;
using System.Linq;
using OrganisationRegistry.Location;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<Location>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.LocationList.Any(item => item.FormattedAddress == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.LocationList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.FormattedAddress == name);
    }
}