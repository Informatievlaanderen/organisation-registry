namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType;

using System;
using System.Linq;
using OrganisationRegistry.MandateRoleType;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<MandateRoleType>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.MandateRoleTypeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.MandateRoleTypeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}