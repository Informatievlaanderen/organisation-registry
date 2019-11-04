namespace OrganisationRegistry.ContactType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class ContactTypeId : GuidValueObject<ContactTypeId>
    {
        public ContactTypeId([JsonProperty("id")] Guid contactTypeId) : base(contactTypeId) { }
    }
}
