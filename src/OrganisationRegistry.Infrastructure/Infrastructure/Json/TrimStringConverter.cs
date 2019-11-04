namespace OrganisationRegistry.Infrastructure.Infrastructure.Json
{
    using System;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    public class TrimStringConverter : JsonConverter
    {
        private static readonly Regex SpaceRemover = new Regex(@"\s+", RegexOptions.Compiled);

        public override bool CanConvert(Type objectType)
            => objectType == typeof(string);

        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            => TrimInputField((string)reader.Value);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");

        public string TrimInputField(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.Trim();
            input = SpaceRemover.Replace(input, " ");

            return input;
        }
    }
}
