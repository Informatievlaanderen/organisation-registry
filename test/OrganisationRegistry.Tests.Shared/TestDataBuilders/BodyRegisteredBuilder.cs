namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using Body.Events;

    public class BodyRegisteredBuilder
    {
        public Guid Id { get; }
        public string Name { get; }
        public string BodyNumber { get; }
        public string ShortName { get; }
        public string Description { get; }
        public DateTime? FormalValidFrom { get; }
        public DateTime? FormalValidTo { get; }

        public BodyRegisteredBuilder(SequentialBodyNumberGenerator sequentialBodyNumberGenerator)
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
            BodyNumber = sequentialBodyNumberGenerator.GenerateNumber();
            ShortName = Name.Substring(0, 2);
            Description = $"{Name}: {ShortName}";
            FormalValidFrom = null;
            FormalValidTo = null;
        }

        public BodyRegistered Build()
            => new(
                Id,
                Name,
                BodyNumber,
                ShortName,
                Description,
                FormalValidFrom,
                FormalValidTo);

        public static implicit operator BodyRegistered(BodyRegisteredBuilder builder)
            => builder.Build();
    }
}
