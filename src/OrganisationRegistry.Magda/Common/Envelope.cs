namespace OrganisationRegistry.Magda.Common
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [Serializable]
    public class Envelope<T>
    {
        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Header? Header { get; set; }

        [XmlElement(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public T? Body { get; set; }
    }
}
