namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using System.Collections.Generic;
    using Organisation;
    using Organisation.Events;

    public class OrganisationCreatedTestDataBuilder
    {
        public OrganisationId Id { get; }
        public string Name { get; }
        public string OvoNumber { get; }
        public string ShortName { get; }
        public string Description { get; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }

        public OrganisationCreatedTestDataBuilder(IOvoNumberGenerator ovoNumberGenerator)
        {
            Id = new OrganisationId(Guid.NewGuid());
            Name = Id.ToString();
            OvoNumber = ovoNumberGenerator.GenerateNumber();
            ShortName = Name.Substring(0, 2);
            Description = $"{Name}: {ShortName}";
            ValidFrom = null;
            ValidTo = null;
        }

        public OrganisationCreatedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationCreated Build()
            => new OrganisationCreated(
                Id,
                Name,
                OvoNumber,
                ShortName,
                Article.None, Description, new List<Purpose>(), false, ValidFrom, ValidTo, null, null);
    }
}
