namespace OrganisationRegistry.Magda.Helpers
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public static class SerializationExtensions
    {
        public static string SerializeObject<T>(this T @object)
        {
            var serializer = new XmlSerializer(@object!.GetType());

            var ns = new XmlSerializerNamespaces();
            ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("zoekonderneming", "http://webservice.zoekondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be");
            ns.Add("geefonderneming", "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be");
            ns.Add("registreerinschrijving", "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be");

            using (var textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, @object, ns);

                return textWriter.ToString();
            }
        }

        public static string SerializeObject<T>(this T @object, IDictionary<string, string> namespaces)
        {
            var serializer = new XmlSerializer(@object!.GetType());

            var ns = new XmlSerializerNamespaces();
            ns.Add("s", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("zoekonderneming", "http://webservice.zoekondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be");
            ns.Add("geefonderneming", "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be");

            using (var textWriter = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, @object, ns);

                return textWriter.ToString();
            }
        }
    }
}
