namespace OrganisationRegistry.BodyClassificationType.Events
{
    using System;
    using Newtonsoft.Json;

    public class BodyClassificationTypeUpdated : BaseEvent<BodyClassificationTypeUpdated>
    {
        public Guid BodyClassificationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        [JsonConstructor]
        public BodyClassificationTypeUpdated(
            Guid bodyClassificationTypeId,
            string name,
            string previousName)
        {
            Id = bodyClassificationTypeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
