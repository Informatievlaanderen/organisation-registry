// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class CreateBuildingRequest
    {
        /// <summary>
        /// Initializes a new instance of the CreateBuildingRequest class.
        /// </summary>
        public CreateBuildingRequest() { }

        /// <summary>
        /// Initializes a new instance of the CreateBuildingRequest class.
        /// </summary>
        public CreateBuildingRequest(System.Guid? id = default(System.Guid?), string name = default(string), int? vimId = default(int?))
        {
            Id = id;
            Name = name;
            VimId = vimId;
        }

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
        [Newtonsoft.Json.JsonProperty(PropertyName = "vimId")]
        public int? VimId { get; set; }

    }
}
