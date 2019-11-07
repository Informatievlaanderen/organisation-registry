namespace OrganisationRegistry.KeyTypes
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class KeyTypeName : StringValueObject<KeyTypeName>
    {
        public KeyTypeName([JsonProperty("name")] string keyTypeName) : base(keyTypeName) { }
    }
}
