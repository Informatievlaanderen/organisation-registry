﻿namespace OrganisationRegistry.Api.FormalFrameworkCategory
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.FormalFrameworkCategory;

    public class UniqueNameValidator : IUniqueNameValidator<FormalFrameworkCategory>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.FormalFrameworkCategoryList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.FormalFrameworkCategoryList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
