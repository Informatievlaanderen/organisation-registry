/*
 * Organisation Registry — local dev seed
 *
 * Munt een AlgemeenBeheerder JWT en maakt alle benodigde parameter types aan
 * via de REST API. Idempotent: bestaande items worden genegeerd.
 * Updatet ook de Api:* sleutels in de SQL Server Configuration tabel.
 */

using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// ---------------------------------------------------------------------------
// Config — overschrijfbaar via env vars
// ---------------------------------------------------------------------------

var apiBase       = Env("API_BASE",       "http://host.docker.internal:9002");
var signingKey    = Env("JWT_SIGNING_KEY", "keycloak-demo-local-dev-secret-key-32b");
var issuer        = Env("JWT_ISSUER",      "organisatieregister");
var audience      = Env("JWT_AUDIENCE",    "organisatieregister");
var developerVoId = Env("DEVELOPER_VO_ID", "9c2f7372-7112-49dc-9771-f127b048b4c7");

var mssqlHost     = Env("MSSQL_HOST",     "mssql");
var mssqlUser     = Env("MSSQL_USER",     "sa");
var mssqlPassword = Env("MSSQL_PASSWORD", "E@syP@ssw0rd");
var mssqlDb       = Env("MSSQL_DB",       "OrganisationRegistry");

var dbConfigKeys = new Dictionary<string, string>
{
    ["Api:Vlimpers_KeyTypeId"]                                                    = "922a46bb-1378-45bd-a61f-b6bbf348a4d5",
    ["Api:Orafin_KeyTypeId"]                                                      = "1e3611a7-7914-411a-a0c9-84fcd6218e67",
    ["Api:Orafin_OvoCode"]                                                        = "OVO000099",
    ["Api:INR_KeyTypeId"]                                                         = "a7e93f01-0001-0000-0000-000000000002",
    ["Api:KBO_KeyTypeId"]                                                         = "a7e93f01-0001-0000-0000-000000000001",
    ["Api:VademecumKeyTypeId"]                                                    = "a7e93f01-0001-0000-0000-000000000003",
    ["Api:VlimpersKort_KeyTypeId"]                                                = "a7e93f01-0001-0000-0000-000000000004",
    ["Api:FrenchLabelTypeId"]                                                     = "a7e93f02-0002-0000-0000-000000000001",
    ["Api:GermanLabelTypeId"]                                                     = "a7e93f02-0002-0000-0000-000000000002",
    ["Api:EnglishLabelTypeId"]                                                    = "a7e93f02-0002-0000-0000-000000000003",
    ["Api:KboV2FormalNameLabelTypeId"]                                            = "a7e93f02-0002-0000-0000-000000000004",
    ["Api:EmailContactTypeId"]                                                    = "a7e93f03-0003-0000-0000-000000000001",
    ["Api:PhoneContactTypeId"]                                                    = "a7e93f03-0003-0000-0000-000000000002",
    ["Api:CellPhoneContactTypeId"]                                                = "a7e93f03-0003-0000-0000-000000000003",
    ["Api:RegisteredOfficeLocationTypeId"]                                        = "a7e93f04-0004-0000-0000-000000000001",
    ["Api:KboV2RegisteredOfficeLocationTypeId"]                                   = "a7e93f04-0004-0000-0000-000000000001",
    ["Api:Bestuursniveau_ClassificationTypeId"]                                   = "a7e93f06-0006-0000-0000-000000000003",
    ["Api:Categorie_ClassificationTypeId"]                                        = "a7e93f06-0006-0000-0000-000000000004",
    ["Api:Entiteitsvorm_ClassificationTypeId"]                                    = "a7e93f06-0006-0000-0000-000000000005",
    ["Api:LegalFormClassificationTypeId"]                                         = "a7e93f06-0006-0000-0000-000000000001",
    ["Api:PolicyDomainClassificationTypeId"]                                      = "a7e93f06-0006-0000-0000-000000000002",
    ["Api:KboV2LegalFormOrganisationClassificationTypeId"]                        = "a7e93f06-0006-0000-0000-000000000001",
    ["Api:Organisatietype_Mandaten_En_Vermogensaangifte_ClassificationTypeId"]    = "94944afb-7261-554c-dac6-a19ad4387359",
};

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------

Console.WriteLine("=== Organisation Registry local dev seed ===");
Console.WriteLine($"  API:   {apiBase}");
Console.WriteLine($"  MSSQL: {mssqlHost}/{mssqlDb}");

Console.WriteLine("\nWachten op SQL Server...");
await WaitForMssqlAsync();

Console.WriteLine("\nWachten op API...");
await WaitForApiAsync();

var token = MintJwt();
Console.WriteLine($"  JWT gemint voor vo_id={developerVoId}");

UpdateDbConfig();

using var http = new HttpClient();
http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
http.DefaultRequestHeaders.Add("Accept", "application/json");

var seed = new Seeder(http, apiBase);
await seed.RunAsync();

// ---------------------------------------------------------------------------
// JWT
// ---------------------------------------------------------------------------

string MintJwt()
{
    var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim("iss",  issuer),
        new Claim("aud",  audience),
        new Claim("sub",  developerVoId),
        new Claim("vo_id", developerVoId),
        new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                  "algemeenBeheerder"),
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
                  "Seed"),
        new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
                  "Script"),
    };

    var jwtToken = new JwtSecurityToken(
        issuer:             issuer,
        audience:           audience,
        claims:             claims,
        notBefore:          DateTime.UtcNow,
        expires:            DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(jwtToken);
}

// ---------------------------------------------------------------------------
// DB config
// ---------------------------------------------------------------------------

void UpdateDbConfig()
{
    Console.WriteLine("\n=== DB config (Configuration tabel) ===");

    var connStr = new SqlConnectionStringBuilder
    {
        DataSource            = mssqlHost,
        UserID                = mssqlUser,
        Password              = mssqlPassword,
        InitialCatalog        = mssqlDb,
        TrustServerCertificate = true,
        Encrypt               = false,
    }.ConnectionString;

    using var conn = new SqlConnection(connStr);
    conn.Open();

    foreach (var (key, value) in dbConfigKeys)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "IF EXISTS (SELECT 1 FROM [OrganisationRegistry].[Configuration] WHERE [Key] = @key) " +
            "    UPDATE [OrganisationRegistry].[Configuration] SET [Value] = @value WHERE [Key] = @key " +
            "ELSE " +
            "    INSERT INTO [OrganisationRegistry].[Configuration] ([Key], [Value]) VALUES (@key, @value)";
        cmd.Parameters.AddWithValue("@key",   key);
        cmd.Parameters.AddWithValue("@value", value);
        cmd.ExecuteNonQuery();
        Console.WriteLine($"  [OK] {key} = {value}");
    }
}

// ---------------------------------------------------------------------------
// Wait helpers
// ---------------------------------------------------------------------------

async Task WaitForMssqlAsync(int maxWaitSeconds = 120)
{
    var connStr = new SqlConnectionStringBuilder
    {
        DataSource             = mssqlHost,
        UserID                 = mssqlUser,
        Password               = mssqlPassword,
        InitialCatalog         = mssqlDb,
        TrustServerCertificate = true,
        Encrypt                = false,
        ConnectTimeout         = 5,
    }.ConnectionString;

    var deadline = DateTime.UtcNow.AddSeconds(maxWaitSeconds);
    while (DateTime.UtcNow < deadline)
    {
        try
        {
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            Console.WriteLine("  SQL Server bereikbaar");
            return;
        }
        catch
        {
            Console.WriteLine($"  Wachten op SQL Server op {mssqlHost} ...");
            await Task.Delay(5000);
        }
    }

    Console.Error.WriteLine("ERROR: SQL Server niet bereikbaar na wachten. Seed mislukt.");
    Environment.Exit(1);
}

async Task WaitForApiAsync(int maxWaitSeconds = 120)
{
    using var client = new HttpClient();
    var deadline = DateTime.UtcNow.AddSeconds(maxWaitSeconds);
    while (DateTime.UtcNow < deadline)
    {
        try
        {
            var resp = await client.GetAsync($"{apiBase}/v1/status");
            if ((int)resp.StatusCode < 500)
            {
                Console.WriteLine($"  API bereikbaar ({(int)resp.StatusCode})");
                return;
            }
        }
        catch { }
        Console.WriteLine($"  Wachten op API op {apiBase}/v1/status ...");
        await Task.Delay(5000);
    }

    Console.Error.WriteLine("ERROR: API niet bereikbaar na wachten. Seed mislukt.");
    Environment.Exit(1);
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

static string Env(string name, string fallback) =>
    Environment.GetEnvironmentVariable(name) is { Length: > 0 } v ? v : fallback;

// ---------------------------------------------------------------------------
// Seeder
// ---------------------------------------------------------------------------

class Seeder
{
    private readonly HttpClient _http;
    private readonly string _apiBase;
    private int _created, _skipped, _errors;
    private readonly Dictionary<string, HashSet<string>> _cache = new Dictionary<string, HashSet<string>>();

    public Seeder(HttpClient http, string apiBase)
    {
        _http = http;
        _apiBase = apiBase;
    }

    public async Task RunAsync()
    {
        await KeyTypesAsync();
        await OrafinOrgAsync();
        await VlimpersOrgAsync();
        await LabelTypesAsync();
        await ContactTypesAsync();
        await LocationTypesAsync();
        await CapacitiesAsync();
        await ClassificationTypesAsync();
        await FormalFrameworkCategoriesAsync();
        await FormalFrameworksAsync();
        await PurposesAsync();
        await LifecyclePhaseTypesAsync();
        await SeatTypesAsync();
        await MandateRoleTypesAsync();
        await OrganisationRelationTypesAsync();

        Console.WriteLine($"\n{"=".PadRight(50, '=')}");
        Console.WriteLine($"Seed voltooid: {_created} aangemaakt, {_skipped} al aanwezig, {_errors} fouten");
        if (_errors > 0)
            Environment.Exit(1);
    }

    // -------------------------------------------------------------------------
    // HTTP helpers
    // -------------------------------------------------------------------------

    async Task<bool> ExistsAsync(string listPath, string id)
    {
        if (!_cache.TryGetValue(listPath, out var set))
        {
            var url = $"{_apiBase}{listPath}?limit=500";
            try
            {
                var json  = await _http.GetStringAsync(url);
                var items = JsonSerializer.Deserialize<JsonArray>(json) ?? new JsonArray();
                set = items
                    .Select(i => i?["id"]?.GetValue<string>()?.ToLowerInvariant() ?? "")
                    .ToHashSet();
            }
            catch
            {
                set = new HashSet<string>();
            }
            _cache[listPath] = set;
        }
        return set.Contains(id.ToLowerInvariant());
    }

    async Task CreateAsync(string label, string path, object body)
    {
        // Extract id via JSON round-trip (avoids dynamic)
        var json = JsonSerializer.Serialize(body);
        var id   = JsonSerializer.Deserialize<JsonObject>(json)?["id"]?.GetValue<string>() ?? "";

        if (await ExistsAsync(path, id))
        {
            Console.WriteLine($"  [bestaat] {label}");
            _skipped++;
            return;
        }

        var resp = await _http.PostAsync(
            $"{_apiBase}{path}",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"  [OK]      {label}");
            _created++;
            _cache.Remove(path);
        }
        else if (resp.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine($"  [bestaat] {label}");
            _skipped++;
        }
        else
        {
            var detail = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"  [FOUT {(int)resp.StatusCode}] {label}: {detail}");
            _errors++;
        }
    }

    async Task PatchAsync(string label, string path, object body)
    {
        var json = JsonSerializer.Serialize(body);
        var resp = await _http.PatchAsync(
            $"{_apiBase}{path}",
            new StringContent(json, Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
            Console.WriteLine($"  [OK]      {label}");
        else
        {
            var detail = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"  [FOUT {(int)resp.StatusCode}] {label}: {detail}");
            _errors++;
        }
    }

    async Task AddKeyAsync(string label, string orgId, string orgKeyId, string keyTypeId, string keyValue)
    {
        var path = $"/v1/organisations/{orgId}/keys";
        if (await ExistsAsync(path, orgKeyId))
        {
            Console.WriteLine($"  [bestaat] {label}");
            _skipped++;
            return;
        }

        var resp = await _http.PostAsync(
            $"{_apiBase}{path}",
            new StringContent(
                JsonSerializer.Serialize(new { organisationKeyId = orgKeyId, keyTypeId, keyValue }),
                Encoding.UTF8, "application/json"));

        if (resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"  [OK]      {label}");
            _created++;
        }
        else if (resp.StatusCode == HttpStatusCode.Conflict)
        {
            Console.WriteLine($"  [bestaat] {label}");
            _skipped++;
        }
        else
        {
            var detail = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"  [FOUT {(int)resp.StatusCode}] {label}: {detail}");
            _errors++;
        }
    }

    // -------------------------------------------------------------------------
    // Seed sections
    // -------------------------------------------------------------------------

    async Task KeyTypesAsync()
    {
        Console.WriteLine("\n=== KeyTypes ===");
        await CreateAsync("Vlimpers",      "/v1/keytypes", new { id = Constants.VlimpersKeyTypeId,                        name = "Vlimpers" });
        await CreateAsync("Orafin",        "/v1/keytypes", new { id = Constants.OrafinKeyTypeId,                          name = "Orafin" });
        await CreateAsync("KBO",           "/v1/keytypes", new { id = "a7e93f01-0001-0000-0000-000000000001",   name = "KBO" });
        await CreateAsync("INR",           "/v1/keytypes", new { id = "a7e93f01-0001-0000-0000-000000000002",   name = "INR" });
        await CreateAsync("Vademecum",     "/v1/keytypes", new { id = "a7e93f01-0001-0000-0000-000000000003",   name = "Vademecum" });
        await CreateAsync("Vlimpers kort", "/v1/keytypes", new { id = "a7e93f01-0001-0000-0000-000000000004",   name = "Vlimpers kort" });
    }

    async Task OrafinOrgAsync()
    {
        Console.WriteLine("\n=== Orafin-organisatie ===");
        await CreateAsync("Orafin (lokale dev)", "/v1/organisations", new
        {
            id                       = Constants.OrafinOrgId,
            name                     = "Orafin",
            ovoNumber                = Constants.OrafinOvoCode,
            showOnVlaamseOverheidSites = false,
        });
        await AddKeyAsync("Orafin key op organisatie",
            Constants.OrafinOrgId, Constants.OrafinOrgKeyId, Constants.OrafinKeyTypeId, "ORAFIN-LOCAL-DEV");
    }

    async Task VlimpersOrgAsync()
    {
        Console.WriteLine("\n=== Vlimpers-testorganisatie ===");
        await CreateAsync("Vlimpers testorg (lokale dev)", "/v1/organisations", new
        {
            id                       = Constants.VlimpersOrgId,
            name                     = "Vlimpers testorganisatie",
            ovoNumber                = Constants.VlimpersOvoCode,
            showOnVlaamseOverheidSites = false,
        });
        await PatchAsync("Vlimpers-beheer inschakelen",
            $"/v1/organisations/{Constants.VlimpersOrgId}/vlimpers",
            new { vlimpersManagement = true });
        await AddKeyAsync("Vlimpers key op organisatie",
            Constants.VlimpersOrgId, Constants.VlimpersOrgKeyId, Constants.VlimpersKeyTypeId, "VLIMPERS-LOCAL-DEV");
    }

    async Task LabelTypesAsync()
    {
        Console.WriteLine("\n=== LabelTypes ===");
        await CreateAsync("Afkorting",            "/v1/labeltypes", new { id = "1955aff4-6df9-da43-6f32-880989e7d210", name = "Afkorting" });
        await CreateAsync("Alternatieve naam",    "/v1/labeltypes", new { id = "b46a2eff-9c78-bbc7-de8f-ce2e7a0f40ce", name = "Alternatieve naam" });
        await CreateAsync("Franse naam",          "/v1/labeltypes", new { id = "a7e93f02-0002-0000-0000-000000000001", name = "Franse naam" });
        await CreateAsync("Duitse naam",          "/v1/labeltypes", new { id = "a7e93f02-0002-0000-0000-000000000002", name = "Duitse naam" });
        await CreateAsync("Engelse naam",         "/v1/labeltypes", new { id = "a7e93f02-0002-0000-0000-000000000003", name = "Engelse naam" });
        await CreateAsync("Formele benaming KBO", "/v1/labeltypes", new { id = "a7e93f02-0002-0000-0000-000000000004", name = "Formele benaming KBO" });
    }

    async Task ContactTypesAsync()
    {
        Console.WriteLine("\n=== ContactTypes ===");
        await CreateAsync("E-mail",   "/v1/contacttypes", new { id = "a7e93f03-0003-0000-0000-000000000001", name = "E-mail",   regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$", example = "info@voorbeeld.be" });
        await CreateAsync("Telefoon", "/v1/contacttypes", new { id = "a7e93f03-0003-0000-0000-000000000002", name = "Telefoon", regex = @"^[0-9\s\+\-\(\)]+$",          example = "+32 2 123 45 67" });
        await CreateAsync("GSM",      "/v1/contacttypes", new { id = "a7e93f03-0003-0000-0000-000000000003", name = "GSM",      regex = @"^[0-9\s\+\-\(\)]+$",          example = "+32 470 12 34 56" });
        await CreateAsync("Website",  "/v1/contacttypes", new { id = "a7e93f03-0003-0000-0000-000000000004", name = "Website",  regex = @"^https?://",                  example = "https://www.voorbeeld.be" });
    }

    async Task LocationTypesAsync()
    {
        Console.WriteLine("\n=== LocationTypes ===");
        await CreateAsync("Maatschappelijke zetel", "/v1/locationtypes", new { id = "a7e93f04-0004-0000-0000-000000000001", name = "Maatschappelijke zetel" });
        await CreateAsync("Operationeel adres",     "/v1/locationtypes", new { id = "a7e93f04-0004-0000-0000-000000000002", name = "Operationeel adres" });
    }

    async Task CapacitiesAsync()
    {
        Console.WriteLine("\n=== Capaciteiten ===");
        await CreateAsync("Voorzitter",     "/v1/capacities", new { id = "f41bc4a3-afc3-5754-c8fa-f9a938a48d88", name = "Voorzitter" });
        await CreateAsync("Secretaris",     "/v1/capacities", new { id = "dfcd7d89-9720-67dd-d425-686cc0139830", name = "Secretaris" });
        await CreateAsync("Lid",            "/v1/capacities", new { id = "a7e93f05-0005-0000-0000-000000000001", name = "Lid" });
        await CreateAsync("Ondervoorzitter","/v1/capacities", new { id = "a7e93f05-0005-0000-0000-000000000002", name = "Ondervoorzitter" });
    }

    async Task ClassificationTypesAsync()
    {
        Console.WriteLine("\n=== OrganisatieclassificatieTypes ===");
        await CreateAsync("Juridische vorm",               "/v1/organisationclassificationtypes", new { id = "a7e93f06-0006-0000-0000-000000000001", name = "Juridische vorm" });
        await CreateAsync("Beleidsdomein",                 "/v1/organisationclassificationtypes", new { id = "a7e93f06-0006-0000-0000-000000000002", name = "Beleidsdomein" });
        await CreateAsync("Bestuursniveau",                "/v1/organisationclassificationtypes", new { id = "a7e93f06-0006-0000-0000-000000000003", name = "Bestuursniveau" });
        await CreateAsync("Categorie",                     "/v1/organisationclassificationtypes", new { id = "a7e93f06-0006-0000-0000-000000000004", name = "Categorie" });
        await CreateAsync("Entiteitsvorm",                 "/v1/organisationclassificationtypes", new { id = "a7e93f06-0006-0000-0000-000000000005", name = "Entiteitsvorm" });
        await CreateAsync("Regelgeving classificatie",     "/v1/organisationclassificationtypes", new { id = "cf2ce6fe-395c-0620-1bab-7152fc4d6f76", name = "Regelgeving classificatie" });
        await CreateAsync("CJM classificatie A",           "/v1/organisationclassificationtypes", new { id = "af8e2f7d-c68c-8c15-c48e-61ef43e5a264", name = "CJM classificatie A" });
        await CreateAsync("CJM classificatie B",           "/v1/organisationclassificationtypes", new { id = "910d6f9e-0427-ea76-c69d-7288107d79f9", name = "CJM classificatie B" });
        await CreateAsync("CJM classificatie C",           "/v1/organisationclassificationtypes", new { id = "c35407e4-8559-08d4-f461-a8247c993d58", name = "CJM classificatie C" });
        await CreateAsync("Mandaten en vermogensaangifte", "/v1/organisationclassificationtypes", new { id = "94944afb-7261-554c-dac6-a19ad4387359", name = "Mandaten en vermogensaangifte" });
    }

    async Task FormalFrameworkCategoriesAsync()
    {
        Console.WriteLine("\n=== FormalFrameworkCategories ===");
        await CreateAsync("Algemeen", "/v1/formalframeworkcategories", new { id = "a7e93f07-0007-0000-0000-000000000001", name = "Algemeen" });
        await CreateAsync("Vlaams",   "/v1/formalframeworkcategories", new { id = "a7e93f07-0007-0000-0000-000000000002", name = "Vlaams" });
        await CreateAsync("Federaal", "/v1/formalframeworkcategories", new { id = "a7e93f07-0007-0000-0000-000000000003", name = "Federaal" });
    }

    async Task FormalFrameworksAsync()
    {
        Console.WriteLine("\n=== FormalFrameworks ===");
        const string ffCat = "a7e93f07-0007-0000-0000-000000000001";
        await CreateAsync("Vlimpers FF 1",    "/v1/formalframeworks", new { id = "38f97d25-eb84-e4ed-f77f-2bf449d53c47", name = "Vlimpers FF 1",    code = "VL-FF-01", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 2",    "/v1/formalframeworks", new { id = "bab9487c-d865-ac39-6ac3-0eb108e4780f", name = "Vlimpers FF 2",    code = "VL-FF-02", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 3",    "/v1/formalframeworks", new { id = "cb43e05c-4003-5d4d-26e0-28696b8570b4", name = "Vlimpers FF 3",    code = "VL-FF-03", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 4",    "/v1/formalframeworks", new { id = "80618743-625b-4fc4-8dba-380cb859f8ad", name = "Vlimpers FF 4",    code = "VL-FF-04", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 5",    "/v1/formalframeworks", new { id = "c73f7759-e13d-41e8-a267-d63f11aae099", name = "Vlimpers FF 5",    code = "VL-FF-05", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 6",    "/v1/formalframeworks", new { id = "e99f0506-d310-1d68-cfe4-2150fcf68e83", name = "Vlimpers FF 6",    code = "VL-FF-06", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 7",    "/v1/formalframeworks", new { id = "a3d184ea-8520-45f0-9e3b-3c7b4b3883d9", name = "Vlimpers FF 7",    code = "VL-FF-07", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 8",    "/v1/formalframeworks", new { id = "f2f536c3-9c22-47d4-81d0-b77a3843be9e", name = "Vlimpers FF 8",    code = "VL-FF-08", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Vlimpers FF 9",    "/v1/formalframeworks", new { id = "59cb11a7-f8a8-45b1-9d6a-38c6960ecc0b", name = "Vlimpers FF 9",    code = "VL-FF-09", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Regelgeving FF 1", "/v1/formalframeworks", new { id = "f03e9a22-8864-49c1-9195-790e4d1fed83", name = "Regelgeving FF 1", code = "RG-FF-01", formalFrameworkCategoryId = ffCat });
        await CreateAsync("Regelgeving FF 2", "/v1/formalframeworks", new { id = "d51abedb-2c71-5da9-abc8-08a673237964", name = "Regelgeving FF 2", code = "RG-FF-02", formalFrameworkCategoryId = ffCat });
    }

    async Task PurposesAsync()
    {
        Console.WriteLine("\n=== Purposes ===");
        await CreateAsync("Vlaamse overheid", "/v1/purposes", new { id = "a7e93f09-0009-0000-0000-000000000001", name = "Vlaamse overheid" });
        await CreateAsync("Lokale overheid",  "/v1/purposes", new { id = "a7e93f09-0009-0000-0000-000000000002", name = "Lokale overheid" });
    }

    async Task LifecyclePhaseTypesAsync()
    {
        Console.WriteLine("\n=== LifecyclePhaseTypes ===");
        await CreateAsync("Actief",   "/v1/lifecyclephasetypes", new { id = "a7e93f0a-000a-0000-0000-000000000001", name = "Actief",   representsActivePhase = true,  isDefaultPhase = true });
        await CreateAsync("Inactief", "/v1/lifecyclephasetypes", new { id = "a7e93f0a-000a-0000-0000-000000000002", name = "Inactief", representsActivePhase = false, isDefaultPhase = false });
    }

    async Task SeatTypesAsync()
    {
        Console.WriteLine("\n=== SeatTypes ===");
        await CreateAsync("Effectief",        "/v1/seattypes", new { id = "a7e93f0b-000b-0000-0000-000000000001", name = "Effectief",        order = 1, isEffective = true });
        await CreateAsync("Plaatsvervangend", "/v1/seattypes", new { id = "a7e93f0b-000b-0000-0000-000000000002", name = "Plaatsvervangend", order = 2, isEffective = false });
    }

    async Task MandateRoleTypesAsync()
    {
        Console.WriteLine("\n=== MandateRoleTypes ===");
        await CreateAsync("Voorzitter", "/v1/mandateroletypes", new { id = "a7e93f0c-000c-0000-0000-000000000001", name = "Voorzitter" });
        await CreateAsync("Secretaris", "/v1/mandateroletypes", new { id = "a7e93f0c-000c-0000-0000-000000000002", name = "Secretaris" });
        await CreateAsync("Lid",        "/v1/mandateroletypes", new { id = "a7e93f0c-000c-0000-0000-000000000003", name = "Lid" });
    }

    async Task OrganisationRelationTypesAsync()
    {
        Console.WriteLine("\n=== OrganisatieRelatieTypes ===");
        await CreateAsync("Is onderdeel van", "/v1/organisationrelationtypes", new { id = "a7e93f0d-000d-0000-0000-000000000001", name = "Is onderdeel van", inverseName = "Bevat" });
        await CreateAsync("Werkt samen met",  "/v1/organisationrelationtypes", new { id = "a7e93f0d-000d-0000-0000-000000000002", name = "Werkt samen met",  inverseName = "Werkt samen met" });
    }
}

// ---------------------------------------------------------------------------
// Constants — moet na alle top-level statements staan (C# 10 beperking)
// ---------------------------------------------------------------------------

static class Constants
{
    public const string OrafinOrgId      = "a7e93f0e-000e-0000-0000-000000000001";
    public const string OrafinOvoCode    = "OVO000099";
    public const string OrafinKeyTypeId  = "1e3611a7-7914-411a-a0c9-84fcd6218e67";
    public const string OrafinOrgKeyId   = "a7e93f0e-000e-0000-0000-000000000002";

    public const string VlimpersOrgId     = "a7e93f0f-000f-0000-0000-000000000001";
    public const string VlimpersOvoCode   = "OVO000002";
    public const string VlimpersKeyTypeId = "922a46bb-1378-45bd-a61f-b6bbf348a4d5";
    public const string VlimpersOrgKeyId  = "a7e93f0f-000f-0000-0000-000000000002";
}
