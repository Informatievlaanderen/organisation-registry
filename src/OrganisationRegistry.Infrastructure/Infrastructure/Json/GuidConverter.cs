namespace OrganisationRegistry.Infrastructure.Infrastructure.Json;

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GuidConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
        => objectType == typeof(Guid);

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var maybeValue = reader.Value;

        if (token is not { })
            return Guid.Empty;

        switch (token.Type)
        {
            case JTokenType.Null:
                return Guid.Empty;

            case JTokenType.String:
                return maybeValue?.ToString() is { } valueToString ? new Guid(valueToString) : Guid.Empty;

            default:
                throw new ArgumentException("Invalid token type");
        }
    }

    public override void WriteJson(JsonWriter writer, object? maybeValue, JsonSerializer serializer)
    {
        if (maybeValue is not { } value || EqualityComparer<Guid>.Default.Equals((Guid)value, default))
        {
            writer.WriteValue(string.Empty);
        }
        else
        {
            writer.WriteValue((Guid)value);
        }
    }
}
