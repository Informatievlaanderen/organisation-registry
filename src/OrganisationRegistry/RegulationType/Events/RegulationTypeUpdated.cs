namespace OrganisationRegistry.RegulationType.Events
{
    using System;

    public class RegulationTypeUpdated : BaseEvent<RegulationTypeUpdated>
    {
        public Guid RegulationTypeId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public RegulationTypeUpdated(
            Guid regulationTypeId,
            string name,
            string previousName)
        {
            Id = regulationTypeId;

            Name = name;
            PreviousName = previousName;
        }
    }
}
