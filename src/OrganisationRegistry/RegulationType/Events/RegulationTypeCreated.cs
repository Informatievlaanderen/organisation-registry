namespace OrganisationRegistry.RegulationType.Events
{
    using System;

    public class RegulationTypeCreated : BaseEvent<RegulationTypeCreated>
    {
        public Guid RegulationTypeId => Id;

        public string Name { get; }

        public RegulationTypeCreated(
            Guid regulationTypeId,
            string name)
        {
            Id = regulationTypeId;
            Name = name;
        }
    }
}
