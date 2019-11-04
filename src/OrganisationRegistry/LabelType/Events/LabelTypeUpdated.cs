namespace OrganisationRegistry.LabelType.Events
{
    using System;

    public class LabelTypeUpdated : BaseEvent<LabelTypeUpdated>
    {
        public Guid LabelTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public LabelTypeUpdated(Guid labelTypeId, string name, string previousName)
        {
            Id = labelTypeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
