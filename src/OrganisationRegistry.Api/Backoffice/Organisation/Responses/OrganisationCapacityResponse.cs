namespace OrganisationRegistry.Api.Backoffice.Organisation.Responses
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SqlServer.Organisation;

    public class OrganisationCapacityResponse
    {
        public Guid OrganisationCapacityId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid CapacityId { get; set; }
        public string CapacityName { get; set; }

        public Guid? PersonId { get; set; }
        public string PersonName { get; set; }

        public Guid? FunctionId { get; set; }
        public string FunctionName { get; set; }

        public Guid? LocationId { get; set; }
        public string LocationName { get; set; }

        public Dictionary<Guid, string> Contacts { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public OrganisationCapacityResponse(OrganisationCapacityListItem organisationCapacity)
        {
            OrganisationCapacityId = organisationCapacity.OrganisationCapacityId;
            OrganisationId = organisationCapacity.OrganisationId;

            CapacityId = organisationCapacity.CapacityId;
            CapacityName = organisationCapacity.CapacityName;

            PersonId = organisationCapacity.PersonId;
            PersonName = organisationCapacity.PersonName;

            FunctionId = organisationCapacity.FunctionId;
            FunctionName = organisationCapacity.FunctionName;

            LocationId = organisationCapacity.LocationId;
            LocationName = organisationCapacity.LocationName;

            Contacts = string.IsNullOrWhiteSpace(organisationCapacity.ContactsJson)
                ? null
                : JsonConvert.DeserializeObject<Dictionary<Guid, string>>(organisationCapacity.ContactsJson);

            ValidFrom = organisationCapacity.ValidFrom;
            ValidTo = organisationCapacity.ValidTo;
        }
    }
}
