namespace OrganisationRegistry.Magda.Responses
{
    using System;
    using System.Xml.Serialization;
    using global::Magda.RegistreerInschrijving;

    [Serializable]
    public class RegistreerInschrijvingResponseBody
    {
        [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
        public RegistreerInschrijvingResponse? RegistreerInschrijvingResponse { get; set; }
    }
}
