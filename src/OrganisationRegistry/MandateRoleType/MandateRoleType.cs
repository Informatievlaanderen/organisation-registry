namespace OrganisationRegistry.MandateRoleType
{
    using Events;
    using Infrastructure.Domain;

    public class MandateRoleType : AggregateRoot
    {
        public MandateRoleTypeName Name { get; private set; }

        private MandateRoleType() { }

        public MandateRoleType(
            MandateRoleTypeId id,
            MandateRoleTypeName name)
        {
            ApplyChange(new MandateRoleTypeCreated(id, name));
        }

        public void Update(MandateRoleTypeName name)
        {
            var @event = new MandateRoleTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(MandateRoleTypeCreated @event)
        {
            Id = @event.MandateRoleTypeId;
            Name = new MandateRoleTypeName(@event.Name);
        }

        private void Apply(MandateRoleTypeUpdated @event)
        {
            Name = new MandateRoleTypeName(@event.Name);
        }
    }
}
