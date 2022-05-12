namespace OrganisationRegistry.Api.Backoffice.Body.Mandate
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.SqlServer.Body;

    public class BodyMandateResponse
    {
        public Guid BodyMandateId { get; set; }
        public BodyMandateType BodyMandateType { get; set; }
        public Guid BodyId { get; set; }

        public Guid BodySeatId { get; set; }
        public string BodySeatNumber { get; set; }
        public string BodySeatName { get; set; }

        public Guid DelegatorId { get; set; }
        public string DelegatorName { get; set; }

        public Guid? DelegatedId { get; set; }
        public string? DelegatedName { get; set; }

        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }

        public Dictionary<Guid, string> Contacts { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public BodyMandateResponse(BodyMandateListItem bodyMandate)
        {
            BodyMandateId = bodyMandate.BodyMandateId;
            BodyMandateType = bodyMandate.BodyMandateType;
            BodyId = bodyMandate.BodyId;

            BodySeatId = bodyMandate.BodySeatId;
            BodySeatNumber = bodyMandate.BodySeatNumber;
            BodySeatName = bodyMandate.BodySeatName;

            DelegatorId = bodyMandate.DelegatorId;
            DelegatorName = bodyMandate.DelegatorName;

            DelegatedId = bodyMandate.DelegatedId;
            DelegatedName = bodyMandate.DelegatedName;

            AssignedToId = bodyMandate.AssignedToId;
            AssignedToName = bodyMandate.AssignedToName;

            Contacts = GetContacts(bodyMandate);

            ValidFrom = bodyMandate.ValidFrom;
            ValidTo = bodyMandate.ValidTo;
        }

        private static Dictionary<Guid, string> GetContacts(BodyMandateListItem bodyMandate)
            => bodyMandate.ContactsJson is { } contactsJson
                ? JsonConvert.DeserializeObject<Dictionary<Guid, string>>(contactsJson)
                  ?? new Dictionary<Guid, string>()
                : new Dictionary<Guid, string>();
    }
}
