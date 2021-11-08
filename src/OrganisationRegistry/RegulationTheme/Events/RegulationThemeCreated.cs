namespace OrganisationRegistry.RegulationTheme.Events
{
    using System;

    public class RegulationThemeCreated : BaseEvent<RegulationThemeCreated>
    {
        public Guid RegulationThemeId => Id;

        public string Name { get; }

        public RegulationThemeCreated(
            Guid regulationThemeId,
            string name)
        {
            Id = regulationThemeId;
            Name = name;
        }
    }
}
