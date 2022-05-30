namespace OrganisationRegistry.Magda.Requests;

using System;
using global::Magda.ZoekOnderneming;
using System.Xml.Serialization;

[Serializable]
public class ZoekOndernemingBody
{
    [XmlElement(Namespace = "http://webservice.zoekondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public ZoekOnderneming ZoekOnderneming { get; set; } = null!;
}