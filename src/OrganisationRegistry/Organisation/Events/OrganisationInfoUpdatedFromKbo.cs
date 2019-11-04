namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationInfoUpdatedFromKbo : BaseEvent<OrganisationInfoUpdatedFromKbo>
    {
        public Guid OrganisationId => Id;

        public string OvoNumber { get; }

        public string Name { get; }
        public string PreviousName { get; }

        public string ShortName { get; }
        public string PreviousShortName { get; }

        public OrganisationInfoUpdatedFromKbo(
            Guid organisationId,
            string ovoNumber,
            string name,
            string shortName,
            string previousName,
            string previousShortName)
        {
            Id = organisationId;

            OvoNumber = ovoNumber;
            Name = name;
            ShortName = shortName;

            PreviousName = previousName;
            PreviousShortName = previousShortName;
        }
    }
}
