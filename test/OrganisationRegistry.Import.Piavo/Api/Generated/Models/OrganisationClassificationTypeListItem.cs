// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class OrganisationClassificationTypeListItem
    {
        /// <summary>
        /// Initializes a new instance of the
        /// OrganisationClassificationTypeListItem class.
        /// </summary>
        public OrganisationClassificationTypeListItem() { }

        /// <summary>
        /// Initializes a new instance of the
        /// OrganisationClassificationTypeListItem class.
        /// </summary>
        public OrganisationClassificationTypeListItem(System.Guid? id = default(System.Guid?), string name = default(string))
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public System.Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}