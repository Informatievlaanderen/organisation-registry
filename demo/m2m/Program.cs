using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Fail fast on missing environment variables
static string RequireEnv(string name)
{
    var value = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrWhiteSpace(value))
        throw new InvalidOperationException($"Required environment variable '{name}' is not set.");
    return value;
}

var keycloakTokenEndpoint = RequireEnv("KEYCLOAK_TOKEN_ENDPOINT");
var cjmClientId           = RequireEnv("CJM_CLIENT_ID");
var cjmClientSecret       = RequireEnv("CJM_CLIENT_SECRET");
var apiBaseUrl            = RequireEnv("API_BASE_URL").TrimEnd('/');
var testOvoId             = RequireEnv("TEST_OVO_ID");

// In-memory token holder (single-instance demo)
string? storedAccessToken = null;

app.MapGet("/", () => Results.Content($$"""
<!DOCTYPE html>
<html lang="nl">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>M2M Demo — Organisation Registry</title>
  <style>
    body { font-family: sans-serif; max-width: 700px; margin: 40px auto; padding: 0 20px; }
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
    pre {
      background: #f4f4f4; border: 1px solid #ddd; border-radius: 4px;
      padding: 16px; white-space: pre-wrap; word-break: break-all; min-height: 80px;
    }
    .label { font-weight: bold; margin-top: 16px; }
  </style>
</head>
<body>
  <h1>Machine-to-Machine Demo — Organisation Registry</h1>
  <p>
    Demonstreert de Client Credentials flow via Keycloak.<br />
    Client: <strong>{{cjmClientId}}</strong> | OVO: <strong>{{testOvoId}}</strong>
  </p>
  <div class="buttons">
    <button id="btn-auth"      onclick="doAction('/demo/authenticate')">Authenticate</button>
    <button id="btn-allowed"   onclick="doAction('/demo/allowed')">Allowed call</button>
    <button id="btn-forbidden" onclick="doAction('/demo/forbidden')">Forbidden call</button>
  </div>
  <div class="label">Resultaat:</div>
  <pre id="result">(nog geen actie uitgevoerd)</pre>

  <script>
    async function doAction(endpoint) {
      const resultEl = document.getElementById('result');
      resultEl.textContent = 'Bezig...';
      document.querySelectorAll('button').forEach(b => b.disabled = true);
      try {
        const res = await fetch(endpoint, { method: 'POST' });
        const data = await res.json();
        resultEl.textContent = JSON.stringify(data, null, 2);
      } catch (err) {
        resultEl.textContent = 'Fout: ' + err.message;
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
    using var http = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Post, keycloakTokenEndpoint)
    {
        Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"]    = "client_credentials",
            ["client_id"]     = cjmClientId,
            ["client_secret"] = cjmClientSecret,
        }),
    };

    var response = await http.SendAsync(request);
    var body     = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
        return Results.Json(
            new { error = "Token request failed", status = (int)response.StatusCode, detail = body },
            statusCode: 502);

    using var doc  = JsonDocument.Parse(body);
    var root       = doc.RootElement;
    storedAccessToken = root.GetProperty("access_token").GetString();

    return Results.Json(new
    {
        authenticated = true,
        expires_in    = root.TryGetProperty("expires_in",  out var exp) ? exp.GetInt32()    : 0,
        scope         = root.TryGetProperty("scope",       out var sc)  ? sc.GetString()    : "",
        token_type    = root.TryGetProperty("token_type",  out var tt)  ? tt.GetString()    : "",
    });
});

app.MapPost("/demo/allowed", async () =>
{
    if (storedAccessToken is null)
        return Results.Json(
            new { error = "Niet geauthenticeerd. Klik eerst op 'Authenticate'." },
            statusCode: 400);

    // Intentionally minimal/invalid body — auth is checked before model validation.
    // 400 = authorised (invalid input), 403 = forbidden, 401 = not authenticated.
    using var http    = new HttpClient();
    var request       = new HttpRequestMessage(HttpMethod.Post,
        $"{apiBaseUrl}/edit/organisations/{testOvoId}/contacts");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedAccessToken);
    request.Content               = new StringContent("{}", Encoding.UTF8, "application/json");

    var response     = await http.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    return Results.Json(new
    {
        status     = (int)response.StatusCode,
        statusText = response.StatusCode.ToString(),
        body       = Truncate(responseBody),
    });
});

app.MapPost("/demo/forbidden", async () =>
{
    if (storedAccessToken is null)
        return Results.Json(
            new { error = "Niet geauthenticeerd. Klik eerst op 'Authenticate'." },
            statusCode: 400);

    // POST /edit/organisations/{ovo}/keys requires dv_organisatieregister_orafinbeheerder.
    // cjmClient only has dv_organisatieregister_cjmbeheerder → expect 403.
    using var http    = new HttpClient();
    var request       = new HttpRequestMessage(HttpMethod.Post,
        $"{apiBaseUrl}/edit/organisations/{testOvoId}/keys");
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", storedAccessToken);
    request.Content               = new StringContent("{}", Encoding.UTF8, "application/json");

    var response     = await http.SendAsync(request);
    var responseBody = await response.Content.ReadAsStringAsync();

    return Results.Json(new
    {
        status     = (int)response.StatusCode,
        statusText = response.StatusCode.ToString(),
        body       = Truncate(responseBody),
    });
});

app.Run();

static string Truncate(string s, int max = 500)
    => s.Length <= max ? s : s[..max] + "…";
