/*
 * Organisation Registry — M2M demo (US2)
 *
 * Demonstreert de machine-to-machine flow met Keycloak client_credentials:
 *
 *  1. orafinClient haalt een token op bij Keycloak
 *  2. GET organisatiesleutels van de Orafin-org → toont huidige waarde
 *  3. PUT om de sleutelwaarde te updaten
 *  4. GET opnieuw → bevestigt de update
 *  5. Probeer een Vlimpers-sleutel aan te passen als Orafin → verwacht 403
 */

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

// ---------------------------------------------------------------------------
// Config — overschrijfbaar via env vars
// ---------------------------------------------------------------------------

var apiBase       = Env("API_BASE",        "http://host.docker.internal:9002");
var keycloakBase  = Env("KEYCLOAK_BASE",   "http://host.docker.internal:8180");
var realm         = Env("KEYCLOAK_REALM",  "organisatieregister");

var orafinClient  = Env("ORAFIN_CLIENT_ID",     "orafinClient");
var orafinSecret  = Env("ORAFIN_CLIENT_SECRET",  "secret");

// Vaste GUIDs — zelfde als in demos/seed/Program.cs (Constants)
var orafinOrgId     = "a7e93f0e-000e-0000-0000-000000000001";
var orafinKeyTypeId = "1e3611a7-7914-411a-a0c9-84fcd6218e67";
var orafinOrgKeyId  = "a7e93f0e-000e-0000-0000-000000000002";

var vlimpersOrgId     = "a7e93f0f-000f-0000-0000-000000000001";
var vlimpersKeyTypeId = "922a46bb-1378-45bd-a61f-b6bbf348a4d5";
var vlimpersOrgKeyId  = "a7e93f0f-000f-0000-0000-000000000002";

var tokenEndpoint = $"{keycloakBase}/realms/{realm}/protocol/openid-connect/token";

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) is { Length: > 0 } v ? v : fallback;

void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 60));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 60));
}

void Ok(string msg)    => Console.WriteLine($"  [OK]      {msg}");
void Info(string msg)  => Console.WriteLine($"  [info]    {msg}");
void Warn(string msg)  => Console.WriteLine($"  [WARN]    {msg}");
void Fail(string msg)  => Console.WriteLine($"  [FOUT]    {msg}");

async Task<string> GetTokenAsync(HttpClient http, string clientId, string clientSecret)
{
    var form = new Dictionary<string, string>
    {
        ["grant_type"]    = "client_credentials",
        ["client_id"]     = clientId,
        ["client_secret"] = clientSecret,
    };

    var resp = await http.PostAsync(tokenEndpoint, new FormUrlEncodedContent(form));
    var body = await resp.Content.ReadAsStringAsync();

    if (!resp.IsSuccessStatusCode)
        throw new Exception($"Token request mislukt ({(int)resp.StatusCode}): {body}");

    var doc   = JsonDocument.Parse(body);
    var token = doc.RootElement.GetProperty("access_token").GetString()
        ?? throw new Exception("Geen access_token in response");

    return token;
}

async Task<(HttpStatusCode status, string body)> ApiGetAsync(HttpClient http, string path)
{
    var resp = await http.GetAsync($"{apiBase}{path}");
    return (resp.StatusCode, await resp.Content.ReadAsStringAsync());
}

async Task<(HttpStatusCode status, string body)> ApiPutAsync(HttpClient http, string path, object payload)
{
    var json = JsonSerializer.Serialize(payload);
    var resp = await http.PutAsync(
        $"{apiBase}{path}",
        new StringContent(json, Encoding.UTF8, "application/json"));
    return (resp.StatusCode, await resp.Content.ReadAsStringAsync());
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------

Console.WriteLine("=== Organisation Registry M2M demo ===");
Console.WriteLine($"  API:      {apiBase}");
Console.WriteLine($"  Keycloak: {keycloakBase} (realm: {realm})");

// Stap 1 — Token ophalen
Section("Stap 1 — orafinClient haalt token op via client_credentials");

using var baseHttp  = new HttpClient();
string orafinToken;
try
{
    orafinToken = await GetTokenAsync(baseHttp, orafinClient, orafinSecret);
    Ok($"Token ontvangen voor client '{orafinClient}'");
    // Toon alleen de eerste 60 tekens om leesbaar te blijven
    Info($"Token (eerste 60 chars): {orafinToken[..Math.Min(60, orafinToken.Length)]}...");
}
catch (Exception ex)
{
    Fail(ex.Message);
    Environment.Exit(1);
    return; // appease compiler
}

// Bouw een HttpClient met de Bearer token
using var http = new HttpClient();
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", orafinToken);
http.DefaultRequestHeaders.Add("Accept", "application/json");

// Stap 2 — GET huidige sleutelwaarde
Section("Stap 2 — GET sleutels van Orafin-organisatie");

var (getStatus, getBody) = await ApiGetAsync(http, $"/v1/organisations/{orafinOrgId}/keys");
Info($"Status: {(int)getStatus} {getStatus}");

string? currentKeyValue = null;
if (getStatus == HttpStatusCode.OK)
{
    try
    {
        var keys = JsonSerializer.Deserialize<JsonArray>(getBody) ?? new JsonArray();
        var orafinKey = keys.FirstOrDefault(k => k?["id"]?.GetValue<string>()
            ?.Equals(orafinOrgKeyId, StringComparison.OrdinalIgnoreCase) == true);

        if (orafinKey != null)
        {
            currentKeyValue = orafinKey["keyValue"]?.GetValue<string>();
            Ok($"Huidige sleutelwaarde: '{currentKeyValue}'");
        }
        else
        {
            Warn($"Sleutel {orafinOrgKeyId} niet gevonden in de response.");
        }
    }
    catch
    {
        Info($"Response body: {getBody[..Math.Min(200, getBody.Length)]}");
    }
}
else
{
    Fail($"Onverwachte status: {(int)getStatus}. Body: {getBody[..Math.Min(200, getBody.Length)]}");
}

// Stap 3 — PUT sleutelwaarde updaten
Section("Stap 3 — PUT om sleutelwaarde te updaten");

var newKeyValue = $"ORAFIN-DEMO-{DateTime.UtcNow:yyyyMMddHHmmss}";
var (putStatus, putBody) = await ApiPutAsync(
    http,
    $"/v1/organisations/{orafinOrgId}/keys/{orafinOrgKeyId}",
    new
    {
        organisationKeyId = orafinOrgKeyId,
        keyTypeId         = orafinKeyTypeId,
        keyValue          = newKeyValue,
    });

Info($"Status: {(int)putStatus} {putStatus}");
if (putStatus == HttpStatusCode.OK || putStatus == HttpStatusCode.NoContent)
    Ok($"Sleutelwaarde bijgewerkt naar '{newKeyValue}'");
else
{
    Fail($"Update mislukt. Body: {putBody[..Math.Min(300, putBody.Length)]}");
    Environment.Exit(1);
}

// Stap 4 — GET om update te bevestigen
Section("Stap 4 — GET om de update te bevestigen");

var (get2Status, get2Body) = await ApiGetAsync(http, $"/v1/organisations/{orafinOrgId}/keys");
Info($"Status: {(int)get2Status} {get2Status}");

if (get2Status == HttpStatusCode.OK)
{
    try
    {
        var keys = JsonSerializer.Deserialize<JsonArray>(get2Body) ?? new JsonArray();
        var orafinKey = keys.FirstOrDefault(k => k?["id"]?.GetValue<string>()
            ?.Equals(orafinOrgKeyId, StringComparison.OrdinalIgnoreCase) == true);

        var updatedValue = orafinKey?["keyValue"]?.GetValue<string>();
        if (updatedValue == newKeyValue)
            Ok($"Bevestigd: sleutelwaarde is nu '{updatedValue}'");
        else
            Warn($"Verwacht '{newKeyValue}', maar zag '{updatedValue}'");
    }
    catch
    {
        Info($"Response body: {get2Body[..Math.Min(200, get2Body.Length)]}");
    }
}
else
{
    Fail($"Onverwachte status: {(int)get2Status}");
}

// Stap 5 — Verboden actie: orafinClient probeert een Vlimpers-sleutel te updaten
Section("Stap 5 — Verboden actie: orafinClient updatet een Vlimpers-sleutel (verwacht 403)");

var (forbidStatus, forbidBody) = await ApiPutAsync(
    http,
    $"/v1/organisations/{vlimpersOrgId}/keys/{vlimpersOrgKeyId}",
    new
    {
        organisationKeyId = vlimpersOrgKeyId,
        keyTypeId         = vlimpersKeyTypeId,
        keyValue          = "ORAFIN-PROBEER-VLIMPERS",
    });

Info($"Status: {(int)forbidStatus} {forbidStatus}");
if (forbidStatus == HttpStatusCode.Forbidden || forbidStatus == HttpStatusCode.Unauthorized)
    Ok($"Correct geweigerd met {(int)forbidStatus} — Orafin mag geen Vlimpers-sleutels aanpassen.");
else if (forbidStatus == HttpStatusCode.OK || forbidStatus == HttpStatusCode.NoContent)
    Fail("BEVEILIGINGSPROBLEEM: de update is onterecht geslaagd!");
else
    Info($"Status {(int)forbidStatus} (verwacht 403). Body: {forbidBody[..Math.Min(200, forbidBody.Length)]}");

// ---------------------------------------------------------------------------
// Samenvatting
// ---------------------------------------------------------------------------

Console.WriteLine();
Console.WriteLine(new string('=', 60));
Console.WriteLine("M2M demo voltooid.");
Console.WriteLine(new string('=', 60));
