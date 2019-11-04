namespace OrganisationRegistry.Organisation.Commands
{
    using System;
    using LabelType;

    public class AddOrganisationLabel : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationLabelId { get; }
        public LabelTypeId LabelTypeId { get; }
        public string LabelValue { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public AddOrganisationLabel(
            Guid organisationLabelId,
            OrganisationId organisationId,
            LabelTypeId labelTypeId,
            string labelValue,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationLabelId = organisationLabelId;
            LabelTypeId = labelTypeId;
            LabelValue = labelValue;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
