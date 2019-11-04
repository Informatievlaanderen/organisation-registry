namespace OrganisationRegistry.Purpose.Events
{
    using System;

    public class PurposeCreated : BaseEvent<PurposeCreated>
    {
        public Guid PurposeId => Id;

        public string Name { get; }

        public PurposeCreated(
            Guid purposeId,
            string name)
        {
            Id = purposeId;

            Name = name;
        }
    }
}
