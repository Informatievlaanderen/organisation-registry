// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    using System;
    using Newtonsoft.Json;

    public partial class FunctionListItem
    {
        /// <summary>
        /// Initializes a new instance of the FunctionListItem class.
        /// </summary>
        public FunctionListItem() { }

        /// <summary>
        /// Initializes a new instance of the FunctionListItem class.
        /// </summary>
        public FunctionListItem(Guid? id = default(Guid?), string name = default(string))
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
