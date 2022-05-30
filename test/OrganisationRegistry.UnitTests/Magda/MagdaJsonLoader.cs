namespace OrganisationRegistry.UnitTests.Magda;

using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrganisationRegistry.Magda.Common;
using OrganisationRegistry.Magda.Responses;

public static class MagdaJsonLoader
{
    public static async Task<Envelope<GeefOndernemingResponseBody>> Load(string kboNr)
    {
        var path = Path.Join("MagdaResponses", $"{kboNr}.json");
        var json = await File.ReadAllTextAsync(path);

        var magdaResponse = JsonConvert.DeserializeObject<Envelope<GeefOndernemingResponseBody>>(json);
        return magdaResponse!;
    }
}