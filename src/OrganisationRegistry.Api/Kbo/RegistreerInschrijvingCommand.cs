namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Cryptography.Xml;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Autofac.Features.OwnedInstances;
    using global::Magda.RegistreerInschrijving;
    using Magda;
    using Magda.Common;
    using Magda.Helpers;
    using Magda.Requests;
    using Magda.Responses;
    using Microsoft.Extensions.Logging;
    using SqlServer.Infrastructure;
    using SqlServer.Magda;

    public interface IRegistreerInschrijvingCommand
    {
        Task<Envelope<RegistreerInschrijvingResponseBody>> Execute(ClaimsPrincipal user, string kboNumber);
    }

    public class RegistreerInschrijvingCommand : IRegistreerInschrijvingCommand
    {
        private readonly MagdaConfiguration _magdaConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RegistreerInschrijvingCommand> _logger;
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        public RegistreerInschrijvingCommand(
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            MagdaConfiguration magdaConfiguration,
            IHttpClientFactory httpClientFactory,
            ILogger<RegistreerInschrijvingCommand> logger)
        {
            _contextFactory = contextFactory;
            _magdaConfiguration = magdaConfiguration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Envelope<RegistreerInschrijvingResponseBody>> Execute(ClaimsPrincipal user, string kboNumber)
        {
            using (var organisationRegistryContext = _contextFactory().Value)
            {
                var reference = await CreateAndStoreReference(organisationRegistryContext, user);
                _logger.LogTrace($"Reference: {reference}");
                return await PerformMagdaRequest<RegistreerInschrijvingResponseBody>(
                    $"{_magdaConfiguration.RepertoriumMagdaEndpoint}/RegistreerInschrijvingDienst-02.01/soap/WebService",
                    SignEnvelope(
                        MakeEnvelope(
                            new RegistreerInschrijvingBody
                            {
                                RegistreerInschrijving = MakeRegistreerInschrijvingRequest(kboNumber, reference)
                            })));
            }
        }

        private async Task<Envelope<T>> PerformMagdaRequest<T>(string endpoint, string signedEnvelope)
        {
            using (var client = _httpClientFactory.CreateClient(MagdaModule.HttpClientName))
            {
                try
                {
                    _logger.LogTrace(signedEnvelope);
                    var response = await client
                        .PostAsync(
                            endpoint,
                            new StringContent(signedEnvelope, Encoding.UTF8, "application/soap+xml"));

                    _logger.LogTrace("RegistreerInschrijving http response {@Result}", response);

                    if (response.IsSuccessStatusCode)
                    {
                        var serializer = new XmlSerializer(typeof(Envelope<T>));

                        using (var reader = new StringReader(await response.Content.ReadAsStringAsync()))
                        {
                            var performMagdaRequest = (Envelope<T>) serializer.Deserialize(reader);
                            return performMagdaRequest;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, ex.Message);
                }

                return null;
            }
        }

        private RegistreerInschrijving MakeRegistreerInschrijvingRequest(string socialSecurityId, string reference) => new RegistreerInschrijving
        {
            Verzoek = new VerzoekType
            {
                Context = new ContextType
                {
                    Naam = "RegistreerInschrijving",
                    Versie = "02.01.0000",
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
                            Identificatie = _magdaConfiguration.Sender,
                            Referte = reference,
                            Hoedanigheid = _magdaConfiguration.RepertoriumCapacity
                        },
                        Ontvanger = new OntvangerAdresType
                        {
                            Identificatie = _magdaConfiguration.Recipient
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
                            Inschrijving = new InschrijvingType
                            {
                                Hoedanigheid = _magdaConfiguration.RepertoriumCapacity,
                                Identificatie = _magdaConfiguration.Sender,
                                BetrokkenSubject = new BetrokkenSubjectType()
                                {
                                    Type = "OND",
                                    Subjecten = new []{ new SubjectType()
                                    {
                                        Sleutel = socialSecurityId,
                                        Type = "OND"
                                    }, }
                                },
                                Periode = new PeriodeVerplichtBeginType
                                {
                                    Begin = DateTime.Now.ToString("yyyy-MM-dd"),
                                    Einde = null
                                }
                            }
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
                _magdaConfiguration.ClientCertificate);

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
