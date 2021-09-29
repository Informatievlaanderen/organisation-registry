namespace OrganisationRegistry.Api.Backoffice.Body.Queries
{
    using System;

    public class BodyContactListQueryResult
    {
        public Guid BodyContactId { get; set; }
        public string ContactTypeName { get; set; }
        public string ContactValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; }

        public BodyContactListQueryResult(
            Guid organisationContactId,
            string contactTypeName,
            string contactValue,
            DateTime? validFrom,
            DateTime? validTo)
        {
            BodyContactId = organisationContactId;
            ContactTypeName = contactTypeName;
            ContactValue = contactValue;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }
}
