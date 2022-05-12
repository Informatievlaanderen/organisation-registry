namespace OrganisationRegistry.Organisation
{
    using System;
    using LabelType;

    public class UpdateOrganisationLabel : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationLabelId { get; }
        public LabelTypeId LabelTypeId { get; }
        public string Value { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public UpdateOrganisationLabel(
            Guid organisationLabelId,
            OrganisationId organisationId,
            LabelTypeId labelTypeId,
            string value,
            ValidFrom validFrom,
            ValidTo validTo)
        {
            Id = organisationId;

            OrganisationLabelId = organisationLabelId;
            LabelTypeId = labelTypeId;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
