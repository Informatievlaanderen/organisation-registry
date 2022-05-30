namespace OrganisationRegistry.Api.Infrastructure.Magda;

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac.Features.OwnedInstances;
using global::Magda.RegistreerInschrijving;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Magda;
using OrganisationRegistry.Magda.Common;
using OrganisationRegistry.Magda.Requests;
using OrganisationRegistry.Magda.Responses;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Magda;

public interface IRegistreerInschrijvingCommand
{
    Task<Envelope<RegistreerInschrijvingResponseBody>?> Execute(IUser user, string kboNumber);
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

    public async Task<Envelope<RegistreerInschrijvingResponseBody>?> Execute(IUser user, string kboNumber)
    {
        using (var organisationRegistryContext = _contextFactory().Value)
        {
            var reference = await CreateAndStoreReference(organisationRegistryContext, user);
            _logger.LogTrace("Reference: {Reference}", reference);
            var unsignedEnvelope = MakeEnvelope(
                new RegistreerInschrijvingBody
                {
                    RegistreerInschrijving = MakeRegistreerInschrijvingRequest(kboNumber, reference)
                });

            var maybeClientCertificate = _magdaConfiguration.ClientCertificate;
            if (maybeClientCertificate is not { } clientCertificate)
                throw new NullReferenceException("ClientCertificate should never be null");

            var signedEnvelope = unsignedEnvelope.SignEnvelope(clientCertificate);

            return await PerformMagdaRequest<RegistreerInschrijvingResponseBody>(
                $"{_magdaConfiguration.RepertoriumMagdaEndpoint}/RegistreerInschrijvingDienst-02.01/soap/WebService",
                signedEnvelope);
        }
    }

    private async Task<Envelope<T>?> PerformMagdaRequest<T>(string endpoint, string signedEnvelope)
    {
        using (var client = _httpClientFactory.CreateClient(MagdaModule.HttpClientName))
        {
            try
            {
                _logger.LogTrace("{SignedEnvelope}", signedEnvelope);
                var response = await client
                    .PostAsync(
                        endpoint,
                        new StringContent(signedEnvelope, Encoding.UTF8, "application/soap+xml"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("RegistreerInschrijving response not successful: {@Result}", response);
                }
                else
                {
                    _logger.LogTrace("RegistreerInschrijving http response: {@Result}", response);

                    var serializer = new XmlSerializer(typeof(Envelope<T>));

                    using (var reader = new StringReader(await response.Content.ReadAsStringAsync()))
                    {
                        return (Envelope<T>?) serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "{Message}", ex.Message);
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
                            BetrokkenSubject = new BetrokkenSubjectType
                            {
                                Type = "OND",
                                Subjecten = new []{ new SubjectType
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
        => new()
        {
            Header = new Header(),
            Body = body
        };
}