namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType;

using System;
using System.Linq;
using OrganisationRegistry.LabelType;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<LabelType>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.KeyTypeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.KeyTypeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}