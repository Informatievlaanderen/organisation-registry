namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification;

using System;
using System.Linq;
using OrganisationRegistry.OrganisationClassification;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameWithinTypeValidator<OrganisationClassification>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name, Guid typeId)
    {
        return
            _context.OrganisationClassificationList
                .AsQueryable()
                .Where(item => item.OrganisationClassificationTypeId == typeId)
                .Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name, Guid typeId)
    {
        return
            _context.OrganisationClassificationList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Where(item => item.OrganisationClassificationTypeId == typeId)
                .Any(item => item.Name == name);
    }
}