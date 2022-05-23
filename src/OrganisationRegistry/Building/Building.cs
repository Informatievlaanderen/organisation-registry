namespace OrganisationRegistry.Building
{
    using Events;
    using Infrastructure.Domain;

    public class Building : AggregateRoot
    {
        public string Name { get; private set; }
        private int? _vimId;

        private Building()
        {
            Name = string.Empty;
        }

        public Building(BuildingId id, string name, int? vimId)
        {
            Name = string.Empty;

            var @event = new BuildingCreated(id, name, vimId);
            ApplyChange(@event);
        }

        public void Update(string name, int? vimId)
        {
            var @event = new BuildingUpdated(Id, name, vimId, Name, _vimId);
            ApplyChange(@event);
        }

        private void Apply(BuildingCreated @event)
        {
            Id = @event.BuildingId;
            Name = @event.Name;
            _vimId = @event.VimId;
        }

        private void Apply(BuildingUpdated @event)
        {
            Name = @event.Name;
            _vimId = @event.VimId;
        }
    }
}
