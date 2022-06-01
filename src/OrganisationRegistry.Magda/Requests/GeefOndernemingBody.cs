namespace OrganisationRegistry.Magda.Requests;

using System;
using global::Magda.GeefOnderneming;
using System.Xml.Serialization;

[Serializable]
public class GeefOndernemingBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOnderneming GeefOnderneming { get; set; } = null!;
}
