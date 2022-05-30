namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification;

using System;
using System.Linq;
using OrganisationRegistry.BodyClassification;
using SqlServer.Infrastructure;

public class UniqueNameValidator : IUniqueNameWithinTypeValidator<BodyClassification>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueNameValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsNameTaken(string name, Guid typeId)
    {
        return
            _context.BodyClassificationList
                .AsQueryable()
                .Where(item => item.BodyClassificationTypeId == typeId)
                .Any(item => item.Name == name);
    }

    public bool IsNameTaken(Guid id, string name, Guid typeId)
    {
        return
            _context.BodyClassificationList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Where(item => item.BodyClassificationTypeId == typeId)
                .Any(item => item.Name == name);
    }
}