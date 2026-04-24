using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

static string RequireEnv(string name)
{
    var value = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException($"Required environment variable '{name}' is not set.");
    return value;
}

var keycloakTokenEndpoint = RequireEnv("KEYCLOAK_TOKEN_ENDPOINT");
var orafinClientId        = RequireEnv("ORAFIN_CLIENT_ID");
var orafinClientSecret    = RequireEnv("ORAFIN_CLIENT_SECRET");
var apiBaseUrl            = RequireEnv("API_BASE_URL").TrimEnd('/');

// In-memory state (single-instance demo)
string? storedAccessToken  = null;
string? storedOrganisationId = null;

app.MapGet("/", () => Results.Content($$"""
<!DOCTYPE html>
<html lang="nl">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>M2M Demo — Organisation Registry</title>
  <style>
    body { font-family: sans-serif; max-width: 860px; margin: 40px auto; padding: 0 20px; }
    h1 { font-size: 1.4rem; }
    .buttons { display: flex; gap: 12px; margin: 24px 0; }
    button {
      padding: 10px 20px; font-size: 1rem; cursor: pointer; border: none;
      border-radius: 4px; color: white;
    }
    #btn-auth      { background: #0066cc; }
    #btn-allowed   { background: #28a745; }
    #btn-forbidden { background: #dc3545; }
    button:disabled { opacity: 0.5; cursor: not-allowed; }
    .panel { margin-top: 20px; }
    .panel-label {
      font-weight: bold; font-size: 0.85rem; text-transform: uppercase;
      letter-spacing: 0.05em; color: #555; margin-bottom: 4px;
    }
    pre {
      background: #f4f4f4; border: 1px solid #ddd; border-radius: 4px;
      padding: 16px; white-space: pre-wrap; word-break: break-all;
      min-height: 60px; font-size: 0.85rem; margin: 0;
    }
    #token-panel { display: none; }
  </style>
</head>
<body>
  <h1>Machine-to-Machine Demo — Organisation Registry</h1>
  <p>
    Demonstreert de Client Credentials flow via Keycloak.<br />
    Client: <strong>{{orafinClientId}}</strong> — heeft scope <code>dv_organisatieregister_orafinbeheerder</code><br />
    <em>Allowed</em>: <code>POST /edit/organisations/{id}/keys</code> (vereist orafinbeheerder of cjmbeheerder)<br />
    <em>Forbidden</em>: <code>POST /edit/organisations/{id}/contacts</code> (vereist cjmbeheerder — orafin heeft die scope niet)
  </p>
  <div class="buttons">
    <button id="btn-auth"      onclick="doAction('/demo/authenticate')">Authenticate</button>
    <button id="btn-allowed"   onclick="doAction('/demo/allowed')">Allowed call</button>
    <button id="btn-forbidden" onclick="doAction('/demo/forbidden')">Forbidden call</button>
  </div>

  <div class="panel" id="token-panel">
    <div class="panel-label">Access token</div>
    <pre id="token-display"></pre>
  </div>

  <div class="panel" style="margin-top:16px">
    <div class="panel-label">Request</div>
    <pre id="request-display">(nog geen actie)</pre>
  </div>

  <div class="panel" style="margin-top:16px">
    <div class="panel-label">Response</div>
    <pre id="result">(nog geen actie)</pre>
  </div>

  <script>
    async function doAction(endpoint) {
      const resultEl  = document.getElementById('result');
      const requestEl = document.getElementById('request-display');
      resultEl.textContent  = 'Bezig...';
      requestEl.textContent = 'Bezig...';
      document.querySelectorAll('button').forEach(b => b.disabled = true);
      try {
        const res  = await fetch(endpoint, { method: 'POST' });
        const data = await res.json();

        if (data.request) {
          requestEl.textContent = JSON.stringify(data.request, null, 2);
          delete data.request;
        } else {
          requestEl.textContent = '(geen request details)';
        }

        if (data.access_token) {
          document.getElementById('token-panel').style.display = 'block';
          document.getElementById('token-display').textContent = data.access_token;
          delete data.access_token;
        }

        resultEl.textContent = JSON.stringify(data, null, 2);
      } catch (err) {
        resultEl.textContent  = 'Fout: ' + err.message;
        requestEl.textContent = '';
      } finally {
        document.querySelectorAll('button').forEach(b => b.disabled = false);
      }
    }
  </script>
</body>
</html>
""", "text/html"));

app.MapPost("/demo/authenticate", async () =>
{
    // Stap 1 — token ophalen als orafinClient
    using var http = new HttpClient();
    var tokenRequest = new HttpRequestMessage(HttpMethod.Post, keycloakTokenEndpoint)
    {
        Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"]    = "client_credentials",
            ["client_id"]     = orafinClientId,
            ["client_secret"] = orafinClientSecret,
        }),
    };

    var tokenResponse = await http.SendAsync(tokenRequest);
    var tokenBody     = await tokenResponse.Content.ReadAsStringAsync();

    if (!tokenResponse.IsSuccessStatusCode)
        return Results.Json(
            new
            {
                error   = "Token request failed",
                request = new { method = "POST", url = keycloakTokenEndpoint, grant_type = "client_credentials", client_id = orafinClientId },
                status  = (int)tokenResponse.StatusCode,
                detail  = tokenBody,
            },
            statusCode: 502);

    using var doc = JsonDocument.Parse(tokenBody);
    var root      = doc.RootElement;
    storedAccessToken = root.GetProperty("access_token").GetString();

    // Stap 2 — eerste organisatie-ID ophalen
    var listUrl = $"{apiBaseUrl}/organisations?limit=1";
    var listRequest = new HttpRequestMessage(HttpMethod.Get, listUrl);
    listRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedAccessToken);
    listRequest.Headers.Add("Accept", "application/json");

    var listResponse = await http.SendAsync(listRequest);
    var listBody     = await listResponse.Content.ReadAsStringAsync();

    storedOrganisationId = null;
    string? organisationName = null;
    try
    {
        var orgs = JsonNode.Parse(listBody)?.AsArray();
        var first = orgs?.Count > 0 ? orgs[0] : null;
        storedOrganisationId = first?["id"]?.GetValue<string>();
        organisationName     = first?["name"]?.GetValue<string>();
    }
    catch { }

    return Results.Json(new
    {
        authenticated    = true,
        expires_in       = root.TryGetProperty("expires_in",  out var exp) ? exp.GetInt32() : 0,
        scope            = root.TryGetProperty("scope",       out var sc)  ? sc.GetString() : "",
        token_type       = root.TryGetProperty("token_type",  out var tt)  ? tt.GetString() : "",
        organisation_id  = storedOrganisationId,
        organisation_name = organisationName,
        access_token     = storedAccessToken,
        request          = new { method = "POST", url = keycloakTokenEndpoint, grant_type = "client_credentials", client_id = orafinClientId },
    });
});

app.MapPost("/demo/allowed", async () =>
{
    if (storedAccessToken is null)
        return Results.Json(new { error = "Niet geauthenticeerd. Klik eerst op 'Authenticate'." }, statusCode: 400);
    if (storedOrganisationId is null)
        return Results.Json(new { error = "Geen organisatie gevonden. Zijn er organisaties in de database?" }, statusCode: 400);

    // orafinClient heeft dv_organisatieregister_orafinbeheerder — Keys policy staat orafin toe
    var url = $"{apiBaseUrl}/edit/organisations/{storedOrganisationId}/keys";

    using var http    = new HttpClient();
    var request       = new HttpRequestMessage(HttpMethod.Post, url);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedAccessToken);
    request.Content               = new StringContent("{}", Encoding.UTF8, "application/json");

    var response     = await http.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    return Results.Json(new
    {
        status     = (int)response.StatusCode,
        statusText = response.StatusCode.ToString(),
        body       = TryParseJson(responseBody),
        request    = new
        {
            method  = "POST",
            url,
            headers = new { Authorization = $"Bearer {storedAccessToken}", ContentType = "application/json" },
            body    = "{}",
        },
    });
});

app.MapPost("/demo/forbidden", async () =>
{
    if (storedAccessToken is null)
        return Results.Json(new { error = "Niet geauthenticeerd. Klik eerst op 'Authenticate'." }, statusCode: 400);
    if (storedOrganisationId is null)
        return Results.Json(new { error = "Geen organisatie gevonden. Zijn er organisaties in de database?" }, statusCode: 400);

    // orafinClient heeft GEEN dv_organisatieregister_cjmbeheerder — Contacts policy verbiedt orafin → 403
    var url = $"{apiBaseUrl}/edit/organisations/{storedOrganisationId}/contacts";

    using var http    = new HttpClient();
    var request       = new HttpRequestMessage(HttpMethod.Post, url);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedAccessToken);
    request.Content               = new StringContent("{}", Encoding.UTF8, "application/json");

    var response     = await http.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    return Results.Json(new
    {
        status     = (int)response.StatusCode,
        statusText = response.StatusCode.ToString(),
        body       = TryParseJson(responseBody),
        request    = new
        {
            method  = "POST",
            url,
            headers = new { Authorization = $"Bearer {storedAccessToken}", ContentType = "application/json" },
            body    = "{}",
        },
    });
});

app.Run();

static object TryParseJson(string s)
{
    try { return JsonDocument.Parse(s).RootElement; }
    catch { return s; }
}
