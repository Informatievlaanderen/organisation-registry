// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class UpdateFormalFrameworkRequest
    {
        /// <summary>
        /// Initializes a new instance of the UpdateFormalFrameworkRequest
        /// class.
        /// </summary>
        public UpdateFormalFrameworkRequest() { }

        /// <summary>
        /// Initializes a new instance of the UpdateFormalFrameworkRequest
        /// class.
        /// </summary>
        public UpdateFormalFrameworkRequest(string name = default(string), string code = default(string), System.Guid? formalFrameworkCategoryId = default(System.Guid?))
        {
            Name = name;
            Code = code;
            FormalFrameworkCategoryId = formalFrameworkCategoryId;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "formalFrameworkCategoryId")]
        public System.Guid? FormalFrameworkCategoryId { get; set; }

    }
}
