namespace OrganisationRegistry.MandateRoleType.Events
{
    using System;

    public class MandateRoleTypeCreated : BaseEvent<MandateRoleTypeCreated>
    {
        public Guid MandateRoleTypeId => Id;

        public string Name { get; }

        public MandateRoleTypeCreated(
            Guid mandateRoleTypeId,
            string name)
        {
            Id = mandateRoleTypeId;
            Name = name;
        }
    }
}
