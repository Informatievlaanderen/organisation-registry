namespace OrganisationRegistry.BodyClassificationType.Events
{
    using System;
    using Newtonsoft.Json;

    public class BodyClassificationTypeCreated : BaseEvent<BodyClassificationTypeCreated>
    {
        public Guid BodyClassificationTypeId => Id;

        public string Name { get; }

        [JsonConstructor]
        public BodyClassificationTypeCreated(
            Guid bodyClassificationTypeId,
            string name)
        {
            Id = bodyClassificationTypeId;
            Name = name;
        }
    }
}
