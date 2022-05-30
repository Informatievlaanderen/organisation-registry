namespace OrganisationRegistry.Api.Infrastructure.Magda;

using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using OrganisationRegistry.Magda.Common;
using OrganisationRegistry.Magda.Helpers;

public static class MagdaXmlSignExtensions
{
    public static string SignEnvelope<T>(this Envelope<T> unsignedEnvelope, X509Certificate2 clientCertificate)
        {
            var unsignedXmlEnvelope = unsignedEnvelope
                .SerializeObject()
                .Replace("<s:Body>", @"<s:Body Id=""Body"">");

            var xmlBody = new XmlDocument();
            xmlBody.LoadXml(unsignedXmlEnvelope);
            var signature = SignXml(
                xmlBody,
                clientCertificate);

            var signedXmlEnvelope = unsignedXmlEnvelope
                .Replace("<s:Header />", $"<s:Header>{signature}</s:Header>");

            var signedXmlEnvelopeDocument = new XmlDocument();
            signedXmlEnvelopeDocument.LoadXml(signedXmlEnvelope);
            return signedXmlEnvelopeDocument.OuterXml;
        }

        private static string SignXml(XmlDocument document, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(document) { SigningKey = cert.GetRSAPrivateKey() };

            // Create a reference to be signed.
            var reference = new Reference { Uri = "#Body" };

            // Add an enveloped transformation to the reference.
            var env = new XmlDsigEnvelopedSignatureTransform(true);
            reference.AddTransform(env);

            //canonicalize
            var c14N = new XmlDsigC14NTransform();
            reference.AddTransform(c14N);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            var keyInfoData = new KeyInfoX509Data(cert);

            var rsaProvider = cert.GetRSAPublicKey();
            var rkv = new RSAKeyValue(rsaProvider);

            keyInfo.AddClause(rkv);
            keyInfo.AddClause(keyInfoData);
            signedXml.KeyInfo = keyInfo;

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            return signedXml.GetXml().OuterXml;
        }
}
