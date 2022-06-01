namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework;

using System;
using System.Linq;
using OrganisationRegistry.FormalFramework;
using SqlServer.Infrastructure;

public class UniqueCodeValidator : IUniqueCodeValidator<FormalFramework>
{
    private readonly OrganisationRegistryContext _context;

    public UniqueCodeValidator(OrganisationRegistryContext context)
    {
        _context = context;
    }

    public bool IsCodeTaken(string name)
    {
        return
            _context.FormalFrameworkList
                .Any(item => item.Code == name);
    }

    public bool IsCodeTaken(Guid id, string name)
    {
        return _context.FormalFrameworkList
            .AsQueryable()
            .Where(item => item.Id != id)
            .Any(item => item.Code == name);
    }
}
