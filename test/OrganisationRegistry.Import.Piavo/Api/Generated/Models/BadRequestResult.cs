// Code generated by Microsoft (R) AutoRest Code Generator 0.17.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace OrganisationRegistry.Import.Piavo.Models
{
    public partial class BadRequestResult
    {
        /// <summary>
        /// Initializes a new instance of the BadRequestResult class.
        /// </summary>
        public BadRequestResult() { }

        /// <summary>
        /// Initializes a new instance of the BadRequestResult class.
        /// </summary>
        public BadRequestResult(int? statusCode = default(int?))
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "statusCode")]
        public int? StatusCode { get; private set; }

    }
}
