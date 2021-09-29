namespace OrganisationRegistry.Api.Backoffice.Organisation.Responses
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using SqlServer.Organisation;

    public class OrganisationFunctionResponse
    {
        public Guid OrganisationFunctionId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid FunctionId { get; set; }
        public string FunctionName { get; set; }

        public Guid PersonId { get; set; }
        public string PersonName { get; set; }

        public Dictionary<Guid, string> Contacts { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public OrganisationFunctionResponse(OrganisationFunctionListItem organisationFunction)
        {
            OrganisationFunctionId = organisationFunction.OrganisationFunctionId;
            OrganisationId = organisationFunction.OrganisationId;

            FunctionId = organisationFunction.FunctionId;
            FunctionName = organisationFunction.FunctionName;

            PersonId = organisationFunction.PersonId;
            PersonName = organisationFunction.PersonName;

            Contacts = string.IsNullOrWhiteSpace(organisationFunction.ContactsJson)
                ? null
                : JsonConvert.DeserializeObject<Dictionary<Guid, string>>(organisationFunction.ContactsJson);

            ValidFrom = organisationFunction.ValidFrom;
            ValidTo = organisationFunction.ValidTo;
        }
    }
}
