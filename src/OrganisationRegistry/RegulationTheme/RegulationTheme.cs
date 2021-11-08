namespace OrganisationRegistry.RegulationTheme
{
    using Events;
    using Infrastructure.Domain;

    public class RegulationTheme : AggregateRoot
    {
        public RegulationThemeName Name { get; private set; }

        private RegulationTheme() { }

        public RegulationTheme(RegulationThemeId id, RegulationThemeName name)
        {
            ApplyChange(new RegulationThemeCreated(id, name));
        }

        public void Update(RegulationThemeName name)
        {
            var @event = new RegulationThemeUpdated(Id, name, Name);
            ApplyChange(@event);
        }

        private void Apply(RegulationThemeCreated @event)
        {
            Id = @event.RegulationThemeId;
            Name = new RegulationThemeName(@event.Name);
        }

        private void Apply(RegulationThemeUpdated @event)
        {
            Name = new RegulationThemeName(@event.Name);
        }
    }
}
