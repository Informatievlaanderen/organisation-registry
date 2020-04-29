﻿namespace OrganisationRegistry.Api.Capacity
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Capacity;

    public class UniqueNameValidator : IUniqueNameValidator<Capacity>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.CapacityList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.CapacityList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
