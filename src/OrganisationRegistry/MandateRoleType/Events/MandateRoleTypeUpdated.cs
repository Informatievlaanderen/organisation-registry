namespace OrganisationRegistry.MandateRoleType.Events
{
    using System;

    public class MandateRoleTypeUpdated : BaseEvent<MandateRoleTypeUpdated>
    {
        public Guid MandateRoleTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public MandateRoleTypeUpdated(
            Guid mandateRoleTypeId,
            string name,
            string previousName)
        {
            Id = mandateRoleTypeId;
            Name = name;
            PreviousName = previousName;
        }
    }
}
