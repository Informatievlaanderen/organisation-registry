namespace OrganisationRegistry.LabelType.Events
{
    using System;

    public class LabelTypeCreated : BaseEvent<LabelTypeCreated>
    {
        public Guid LabelTypeId => Id;

        public string Name { get; }

        public LabelTypeCreated(Guid labelTypeId, string name)
        {
            Id = labelTypeId;
            Name = name;
        }
    }
}
