// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class UpdateFunctionRequest
    {
        /// <summary>
        /// Initializes a new instance of the UpdateFunctionRequest class.
        /// </summary>
        public UpdateFunctionRequest() { }

        /// <summary>
        /// Initializes a new instance of the UpdateFunctionRequest class.
        /// </summary>
        public UpdateFunctionRequest(string name = default(string))
        {
            Name = name;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

    }
}
