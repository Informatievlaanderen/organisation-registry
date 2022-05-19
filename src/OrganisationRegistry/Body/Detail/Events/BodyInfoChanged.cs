namespace OrganisationRegistry.Body.Events
{
    using System;

    public class BodyInfoChanged : BaseEvent<BodyInfoChanged>
    {
        public Guid BodyId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public string? ShortName { get; }
        public string? PreviousShortName { get; }

        public string? Description { get; }
        public string? PreviousDescription { get; }

        public BodyInfoChanged(
            Guid bodyId,
            string name,
            string? shortName,
            string? description,
            string previousName,
            string? previousShortName,
            string? previousDescription)
        {
            Id = bodyId;

            Name = name;
            ShortName = shortName;
            Description = description;

            PreviousName = previousName;
            PreviousShortName = previousShortName;
            PreviousDescription = previousDescription;
        }
    }
}
