namespace OrganisationRegistry.RegulationTheme.Events
{
    using System;

    public class RegulationThemeUpdated : BaseEvent<RegulationThemeUpdated>
    {
        public Guid RegulationThemeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public RegulationThemeUpdated(
            Guid regulationThemeId,
            string name,
            string previousName)
        {
            Id = regulationThemeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
