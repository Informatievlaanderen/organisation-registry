namespace OrganisationRegistry.RegulationSubTheme.Events
{
    using System;
    using Newtonsoft.Json;
    using RegulationTheme;

    public class RegulationSubThemeUpdated : BaseEvent<RegulationSubThemeUpdated>
    {
        public Guid RegulationSubThemeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public int Order { get; }
        public int PreviousOrder { get; }

        public string ExternalKey { get; }
        public string PreviousExternalKey { get; }

        public bool Active { get; }
        public bool PreviousActive { get; }

        public Guid RegulationThemeId { get; }
        public Guid PreviousRegulationThemeId { get; }

        public string RegulationThemeName { get; }
        public string PreviousRegulationThemeName { get; }

        public RegulationSubThemeUpdated(
            RegulationSubThemeId regulationSubThemeId,
            RegulationSubThemeName name,
            RegulationThemeId regulationThemeId,
            RegulationThemeName regulationThemeName,
            RegulationSubThemeName previousName,
            RegulationThemeId previousRegulationThemeId,
            RegulationThemeName previousRegulationThemeName)
        {
            Id = regulationSubThemeId;

            Name = name;
            RegulationThemeId = regulationThemeId;
            RegulationThemeName = regulationThemeName;

            PreviousName = previousName;
            PreviousRegulationThemeId = previousRegulationThemeId;
            PreviousRegulationThemeName = previousRegulationThemeName;
        }

        [JsonConstructor]
        public RegulationSubThemeUpdated(
            Guid regulationSubThemeId,
            string name,
            Guid regulationThemeId,
            string regulationThemeName,
            string previousName,
            Guid previousRegulationThemeId,
            string previousRegulationThemeName)
            : this(
                new RegulationSubThemeId(regulationSubThemeId),
                new RegulationSubThemeName(name),
                new RegulationThemeId(regulationThemeId),
                new RegulationThemeName(regulationThemeName),
                new RegulationSubThemeName(previousName),
                new RegulationThemeId(previousRegulationThemeId),
                new RegulationThemeName(previousRegulationThemeName)) { }
    }
}
