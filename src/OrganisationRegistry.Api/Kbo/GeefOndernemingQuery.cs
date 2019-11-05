namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Configuration;
    using global::Magda.GeefOnderneming;
    using Magda;
    using Magda.Common;
    using Magda.Helpers;
    using Magda.Requests;
    using Magda.Responses;
    using SqlServer.Infrastructure;
    using SqlServer.Magda;

    public class GeefOndernemingQuery
    {
        private readonly MagdaConfiguration _configuration;
        private readonly Func<OrganisationRegistryContext> _context;

        public GeefOndernemingQuery(MagdaConfiguration configuration, Func<OrganisationRegistryContext> context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<Envelope<GeefOndernemingResponseBody>> Execute(ClaimsPrincipal user, string kboNumberDotLess)
        {
            var reference = await CreateAndStoreReference(_context(), user);
            return await PerformMagdaRequest<GeefOndernemingResponseBody>(
                $"{_configuration.KBOMagdaEndPoint}/GeefOndernemingDienst-02.00/soap/WebService",
                SignEnvelope(
                    MakeEnvelope(
                        new GeefOndernemingBody
                        {
                            GeefOnderneming = MakeGeefOndernemingRequest(kboNumberDotLess, reference)
                        })));
        }

        private async Task<Envelope<T>> PerformMagdaRequest<T>(string endpoint, string signedEnvelope)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = SslProtocols.Tls12;
                handler.ClientCertificates.Add(
                    _configuration.ClientCertificate);

                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(_configuration.Timeout);

                    try
                    {
                        var response = client
                            .PostAsync(
                                endpoint,
                                new StringContent(signedEnvelope, Encoding.UTF8, "application/soap+xml"))
                            .Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var serializer = new XmlSerializer(typeof(Envelope<T>));

                            using (var reader = new StringReader(await response.Content.ReadAsStringAsync()))
                            {
                                return (Envelope<T>)serializer.Deserialize(reader);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            return null;
        }

        private GeefOnderneming MakeGeefOndernemingRequest(string kboNumber, string reference) => new GeefOnderneming
        {
            Verzoek = new VerzoekType
            {
                Context = new ContextType
                {
                    Naam = "GeefOnderneming",
                    Versie = "02.00.0000",
                    Bericht = new BerichtType
                    {
                        Type = BerichtTypeType.VRAAG,
                        Tijdstip = new TijdstipType
                        {
                            Datum = DateTime.Now.ToString("yyyy-MM-dd"),
                            Tijd = DateTime.Now.ToString("HH:mm:ss.000")
                        },
                        Afzender = new AfzenderAdresType
                        {
                            Identificatie = _configuration.Sender,
                            Referte = reference
                        },
                        Ontvanger = new OntvangerAdresType
                        {
                            Identificatie = _configuration.Recipient
                        }
                    }
                },
                Vragen = new VragenType
                {
                    Vraag = new VraagType
                    {
                        Referte = reference,
                        Inhoud = new VraagInhoudType
                        {
                            Criteria = new CriteriaGeefOnderneming2_0Type
                            {
                                Basisgegevens = VlagEnumType.Item1,
                                Ondernemingsnummer = kboNumber,
                                Rechtstoestanden = VlagEnumType.Item1,
                                Vestigingen = new CriteriaVestigingenType
                                {
                                    Aanduiding = VlagEnumType.Item1,
                                    Details = VlagEnumType.Item1
                                },
                                Bankrekeningen = VlagEnumType.Item1,
                                Functies = new CriteriaFunctiesType
                                {
                                    Aanduiding = VlagEnumType.Item1,
                                    Onderneming = VlagEnumType.Item1
                                },
                                Activiteiten = VlagEnumType.Item1,
                                GerelateerdeOndernemingen = new CriteriaGerelateerdeOndernemingenType
                                {
                                    Aanduiding = VlagEnumType.Item1,
                                    Vestigingen = VlagEnumType.Item1
                                },
                                Hoedanigheden = new CriteriaHoedanighedenType
                                {
                                    Aanduiding = VlagEnumType.Item1,
                                },
                                ExterneIdentificaties = VlagEnumType.Item1,
                                Bijhuis = VlagEnumType.Item1,
                                AmbtshalveDoorhalingen = VlagEnumType.Item1,
                                Omschrijvingen = new CriteriaOmschrijvingenType
                                {
                                    Aanduiding = VlagEnumType.Item1
                                }
                            },
                            Taal = "nl"
                        }
                    }
                }
            }
        };

        private static async Task<string> CreateAndStoreReference(OrganisationRegistryContext context, ClaimsPrincipal principal)
        {
            var magdaCallReference = new MagdaCallReference
            {
                Reference = Guid.NewGuid(),
                CalledAt = DateTimeOffset.UtcNow,
                UserClaims = string.Join(Environment.NewLine, principal.Claims.Select(claim => $"{claim.Type}: {claim.Value}"))
            };
            context.MagdaCallReferences.Add(magdaCallReference);
            await context.SaveChangesAsync();
            return magdaCallReference.Reference.ToString("D");
        }

        private static Envelope<T> MakeEnvelope<T>(T body)
        {
            return new Envelope<T>
            {
                Header = new Header(),
                Body = body
            };
        }

        private string SignEnvelope<T>(Envelope<T> unsignedEnvelope)
        {
            var unsignedXmlEnvelope = unsignedEnvelope
                .SerializeObject()
                .Replace("<s:Body>", @"<s:Body Id=""Body"">");

            var xmlBody = new XmlDocument();
            xmlBody.LoadXml(unsignedXmlEnvelope);
            var signature = SignXml(
                xmlBody,
                _configuration.ClientCertificate);

            var signedXmlEnvelope = unsignedXmlEnvelope
                .Replace("<s:Header />", $"<s:Header>{signature}</s:Header>");

            var signedXmlEnvelopeDocument = new XmlDocument();
            signedXmlEnvelopeDocument.LoadXml(signedXmlEnvelope);
            return signedXmlEnvelopeDocument.OuterXml;
        }

        private static string SignXml(XmlDocument document, X509Certificate2 cert)
        {
            var signedXml = new SignedXml(document) { SigningKey = cert.PrivateKey };

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
            //var kin = new KeyInfoName { Value = "Public key of certificate" };

            var rsaprovider = (RSA)cert.PublicKey.Key;
            var rkv = new RSAKeyValue(rsaprovider);
            //keyInfo.AddClause(kin);
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
}
