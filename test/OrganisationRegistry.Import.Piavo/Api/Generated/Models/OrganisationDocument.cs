// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class OrganisationDocument
    {
        /// <summary>
        /// Initializes a new instance of the OrganisationDocument class.
        /// </summary>
        public OrganisationDocument() { }

        /// <summary>
        /// Initializes a new instance of the OrganisationDocument class.
        /// </summary>
        public OrganisationDocument(int? changeId = default(int?), System.DateTime? changeTime = default(System.DateTime?), System.Guid? id = default(System.Guid?), string name = default(string), string ovoNumber = default(string), string shortName = default(string), Period validity = default(Period), string description = default(string), bool? showOnVlaamseOverheidSites = default(bool?), System.Collections.Generic.IList<Purpose> purposes = default(System.Collections.Generic.IList<Purpose>), System.Collections.Generic.IList<OrganisationLabel> labels = default(System.Collections.Generic.IList<OrganisationLabel>), System.Collections.Generic.IList<OrganisationKey> keys = default(System.Collections.Generic.IList<OrganisationKey>), System.Collections.Generic.IList<OrganisationContact> contacts = default(System.Collections.Generic.IList<OrganisationContact>), System.Collections.Generic.IList<OrganisationOrganisationClassification> organisationClassifications = default(System.Collections.Generic.IList<OrganisationOrganisationClassification>), System.Collections.Generic.IList<OrganisationFunction> functions = default(System.Collections.Generic.IList<OrganisationFunction>), System.Collections.Generic.IList<OrganisationRelation> relations = default(System.Collections.Generic.IList<OrganisationRelation>), System.Collections.Generic.IList<OrganisationCapacity> capacities = default(System.Collections.Generic.IList<OrganisationCapacity>), System.Collections.Generic.IList<OrganisationParent> parents = default(System.Collections.Generic.IList<OrganisationParent>), System.Collections.Generic.IList<OrganisationFormalFramework> formalFrameworks = default(System.Collections.Generic.IList<OrganisationFormalFramework>), System.Collections.Generic.IList<OrganisationBuilding> buildings = default(System.Collections.Generic.IList<OrganisationBuilding>), System.Collections.Generic.IList<OrganisationLocation> locations = default(System.Collections.Generic.IList<OrganisationLocation>), System.Collections.Generic.IList<OrganisationBody> bodies = default(System.Collections.Generic.IList<OrganisationBody>), System.Collections.Generic.IList<OrganisationBankAccount> bankAccounts = default(System.Collections.Generic.IList<OrganisationBankAccount>))
        {
            ChangeId = changeId;
            ChangeTime = changeTime;
            Id = id;
            Name = name;
            OvoNumber = ovoNumber;
            ShortName = shortName;
            Validity = validity;
            Description = description;
            ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;
            Purposes = purposes;
            Labels = labels;
            Keys = keys;
            Contacts = contacts;
            OrganisationClassifications = organisationClassifications;
            Functions = functions;
            Relations = relations;
            Capacities = capacities;
            Parents = parents;
            FormalFrameworks = formalFrameworks;
            Buildings = buildings;
            Locations = locations;
            Bodies = bodies;
            BankAccounts = bankAccounts;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "changeId")]
        public int? ChangeId { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "changeTime")]
        public System.DateTime? ChangeTime { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ovoNumber")]
        public string OvoNumber { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "shortName")]
        public string ShortName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "validity")]
        public Period Validity { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "showOnVlaamseOverheidSites")]
        public bool? ShowOnVlaamseOverheidSites { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "purposes")]
        public System.Collections.Generic.IList<Purpose> Purposes { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "labels")]
        public System.Collections.Generic.IList<OrganisationLabel> Labels { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "keys")]
        public System.Collections.Generic.IList<OrganisationKey> Keys { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "contacts")]
        public System.Collections.Generic.IList<OrganisationContact> Contacts { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "organisationClassifications")]
        public System.Collections.Generic.IList<OrganisationOrganisationClassification> OrganisationClassifications { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "functions")]
        public System.Collections.Generic.IList<OrganisationFunction> Functions { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "relations")]
        public System.Collections.Generic.IList<OrganisationRelation> Relations { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "capacities")]
        public System.Collections.Generic.IList<OrganisationCapacity> Capacities { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "parents")]
        public System.Collections.Generic.IList<OrganisationParent> Parents { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "formalFrameworks")]
        public System.Collections.Generic.IList<OrganisationFormalFramework> FormalFrameworks { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "buildings")]
        public System.Collections.Generic.IList<OrganisationBuilding> Buildings { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "locations")]
        public System.Collections.Generic.IList<OrganisationLocation> Locations { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bodies")]
        public System.Collections.Generic.IList<OrganisationBody> Bodies { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "bankAccounts")]
        public System.Collections.Generic.IList<OrganisationBankAccount> BankAccounts { get; set; }

    }
}
