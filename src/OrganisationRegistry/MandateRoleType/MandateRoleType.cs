namespace OrganisationRegistry.MandateRoleType
{
    using Events;
    using Infrastructure.Domain;

    public class MandateRoleType : AggregateRoot
    {
        public string Name { get; private set; }

        private MandateRoleType() { }

        public MandateRoleType(MandateRoleTypeId id, string name)
        {
            ApplyChange(new MandateRoleTypeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new MandateRoleTypeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(MandateRoleTypeCreated @event)
        {
            Id = @event.MandateRoleTypeId;
            Name = @event.Name;
        }

        private void Apply(MandateRoleTypeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
