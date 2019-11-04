namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationCoupledWithKbo : BaseEvent<OrganisationCoupledWithKbo>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public DateTime? ValidFrom { get; }
        public string KboNumber { get; }

        public OrganisationCoupledWithKbo(
            Guid organisationId,
            string kboNumber,
            string name,
            string ovoNumber,
            DateTime? validFrom)
        {
            Id = organisationId;

            KboNumber = kboNumber;
            Name = name;
            OvoNumber = ovoNumber;
            ValidFrom = validFrom;
        }
    }
}
