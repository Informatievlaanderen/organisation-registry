namespace OrganisationRegistry.Magda.Responses;

using System;
using global::Magda.GeefOnderneming;
using System.Xml.Serialization;

[Serializable]
public class GeefOndernemingResponseBody
{
    [XmlElement(Namespace = "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be")]
    public GeefOndernemingResponse? GeefOndernemingResponse { get; set; }
}
