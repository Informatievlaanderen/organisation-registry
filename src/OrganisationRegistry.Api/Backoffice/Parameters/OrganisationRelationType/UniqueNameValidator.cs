namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType;

using System;
using System.Linq;
using OrganisationRegistry.OrganisationRelationType;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameValidator<OrganisationRelationType>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name)
    {
        return _context.OrganisationRelationTypeList.Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.OrganisationRelationTypeList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Name == name);
    }
}
