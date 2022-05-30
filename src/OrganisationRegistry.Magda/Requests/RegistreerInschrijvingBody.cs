namespace OrganisationRegistry.Magda.Requests;

using System;
using System.Xml.Serialization;
using global::Magda.RegistreerInschrijving;

[Serializable]
public class RegistreerInschrijvingBody
{
    [XmlElement(Namespace = "http://webservice.registreerinschrijvingdienst-02_01.repertorium-02_01.vip.vlaanderen.be")]
    public RegistreerInschrijving RegistreerInschrijving { get; set; } = null!;
}