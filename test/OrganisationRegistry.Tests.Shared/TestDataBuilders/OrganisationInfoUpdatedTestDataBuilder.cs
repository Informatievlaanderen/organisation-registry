namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using System.Collections.Generic;
    using Organisation.Events;

    public class OrganisationInfoUpdatedTestDataBuilder
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Number { get; private set; }
        public string ShortName { get; private set; }
        public string Description { get; private set; }
        public DateTime? ValidFrom { get; private set; }
        public DateTime? ValidTo { get; private set; }
        public string PreviousName { get; private set; }
        public string PreviousNumber { get; private set; }
        public string PreviousShortName { get; private set; }
        public string PreviousDescription { get; private set; }
        public DateTime? PreviouslyValidFrom { get; private set; }
        public DateTime? PreviouslyValidTo { get; private set; }

        public OrganisationInfoUpdatedTestDataBuilder(OrganisationCreated created)
        {
            Id = created.OrganisationId;
            Name = created.Name;
            Number = created.OvoNumber;
            ShortName = created.ShortName;
            Description = created.Description;
            ValidFrom = created.ValidFrom;
            ValidTo = created.ValidTo;

            PreviousName = created.Name;
            PreviousNumber = created.OvoNumber;
            PreviousShortName = created.ShortName;
            PreviousDescription = created.Description;
            PreviouslyValidFrom = created.ValidFrom;
            PreviouslyValidTo = created.ValidTo;
        }

        public OrganisationInfoUpdatedTestDataBuilder WithValidity(DateTime? from, DateTime? to)
        {
            ValidFrom = from;
            ValidTo = to;
            return this;
        }

        public OrganisationInfoUpdatedTestDataBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public OrganisationInfoUpdated Build()
            => new OrganisationInfoUpdated(
                Id,
                Name,
                Description,
                Number,
                ShortName,
                new List<Purpose>(),
                false,
                ValidFrom,
                ValidTo,
                PreviousName,
                PreviousDescription,
                PreviousShortName,
                new List<Purpose>(),
                false,
                PreviouslyValidFrom,
                PreviouslyValidTo);
    }
}
