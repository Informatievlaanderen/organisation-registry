namespace OrganisationRegistry.RegulationTheme
{
    using Events;
    using Infrastructure.Domain;

    public class RegulationTheme : AggregateRoot
    {
        public string Name { get; private set; }

        private RegulationTheme() { }

        public RegulationTheme(RegulationThemeId id, string name)
        {
            ApplyChange(new RegulationThemeCreated(id, name));
        }

        public void Update(string name)
        {
            var @event = new RegulationThemeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(RegulationThemeCreated @event)
        {
            Id = @event.RegulationThemeId;
            Name = @event.Name;
        }

        private void Apply(RegulationThemeUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
