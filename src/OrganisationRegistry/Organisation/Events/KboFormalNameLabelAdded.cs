namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class KboFormalNameLabelAdded : BaseEvent<KboFormalNameLabelAdded>
    {
        public Guid OrganisationId => Id;

        public Guid OrganisationLabelId { get; }
        public Guid LabelTypeId { get; }
        public string LabelTypeName { get; }
        public string Value { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public KboFormalNameLabelAdded(
            Guid organisationId,
            Guid organisationLabelId,
            Guid labelTypeId,
            string labelTypeName,
            string value,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = organisationId;

            OrganisationLabelId = organisationLabelId;
            LabelTypeId = labelTypeId;
            LabelTypeName = labelTypeName;
            Value = value;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
