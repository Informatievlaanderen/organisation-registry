namespace OrganisationRegistry.Api.Infrastructure.Magda;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac.Features.OwnedInstances;
using global::Magda.GeefOnderneming;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Magda;
using OrganisationRegistry.Magda.Common;
using OrganisationRegistry.Magda.Requests;
using OrganisationRegistry.Magda.Responses;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Magda;

public interface IGeefOndernemingQuery
{
    Task<Envelope<GeefOndernemingResponseBody>?> Execute(IUser user, string kboNumberDotLess);
}

public class GeefOndernemingQuery : IGeefOndernemingQuery
{
    private readonly MagdaConfiguration _configuration;
    private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GeefOndernemingQuery> _logger;

    public GeefOndernemingQuery(
        MagdaConfiguration configuration,
        Func<Owned<OrganisationRegistryContext>> contextFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<GeefOndernemingQuery> logger)
    {
        _configuration = configuration;
        _contextFactory = contextFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<Envelope<GeefOndernemingResponseBody>?> Execute(IUser user, string kboNumberDotLess)
    {
        using (var organisationRegistryContext = _contextFactory().Value)
        {
            var reference = await CreateAndStoreReference(organisationRegistryContext, user);

            var unsignedEnvelope = MakeEnvelope(
                new GeefOndernemingBody
                {
                    GeefOnderneming = MakeGeefOndernemingRequest(kboNumberDotLess, reference)
                });

            var maybeClientCertificate = _configuration.ClientCertificate;
            if (maybeClientCertificate is not { } clientCertificate)
                throw new NullReferenceException("ClientCertificate should never be null");

            var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

            return await PerformMagdaRequest<GeefOndernemingResponseBody>(
                $"{_configuration.KBOMagdaEndPoint}/GeefOndernemingDienst-02.00/soap/WebService",
                signedEnvelope);
        }
    }

    private async Task<Envelope<T>?> PerformMagdaRequest<T>(string endpoint, string signedEnvelope)
    {
        using (var client = _httpClientFactory.CreateClient(MagdaModule.HttpClientName))
        {
            client.Timeout = TimeSpan.FromSeconds(_configuration.Timeout);

            try
            {
                var response = await client
                    .PostAsync(
                        endpoint,
                        new StringContent(signedEnvelope, Encoding.UTF8, "application/soap+xml"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GeefOnderneming response not successful: {@Result}", response);
                }
                else
                {
                    _logger.LogTrace("GeefOnderneming http response: {@Result}", response);

                    var serializer = new XmlSerializer(typeof(Envelope<T>));

                    using (var reader = new StringReader(await response.Content.ReadAsStringAsync()))
                    {
                        return (Envelope<T>?) serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
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
                            RechtstoestandenSpecified = true,
                            Vestigingen = new CriteriaVestigingenType
                            {
                                Aanduiding = VlagEnumType.Item1,
                                Details = VlagEnumType.Item1
                            },
                            Bankrekeningen = VlagEnumType.Item1,
                            BankrekeningenSpecified = true,
                            Functies = new CriteriaFunctiesType
                            {
                                Aanduiding = VlagEnumType.Item1,
                                Onderneming = VlagEnumType.Item1
                            },
                            Activiteiten = VlagEnumType.Item1,
                            ActiviteitenSpecified = true,
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
                            ExterneIdentificatiesSpecified = true,
                            Bijhuis = VlagEnumType.Item1,
                            BijhuisSpecified = true,
                            AmbtshalveDoorhalingen = VlagEnumType.Item1,
                            AmbtshalveDoorhalingenSpecified = true,
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

    private static async Task<string> CreateAndStoreReference(OrganisationRegistryContext context, IUser user)
    {
        var magdaCallReference = new MagdaCallReference
        {
            Reference = Guid.NewGuid(),
            CalledAt = DateTimeOffset.UtcNow,
            UserClaims = $"FirstName: {user.FirstName} | " +
                         $"LastName: {user.LastName} | " +
                         $"UserId: {user.UserId} | " +
                         $"Ip: {user.Ip} | " +
                         $"Roles: {string.Join(',', user.Roles)}"
        };
        await context.MagdaCallReferences.AddAsync(magdaCallReference);
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
}
