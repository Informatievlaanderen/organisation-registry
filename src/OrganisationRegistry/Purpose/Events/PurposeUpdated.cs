namespace OrganisationRegistry.Purpose.Events
{
    using System;

    public class PurposeUpdated : BaseEvent<PurposeUpdated>
    {
        public Guid PurposeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public PurposeUpdated(
            Guid purposeId,
            string name,
            string previousName)
        {
            Id = purposeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
