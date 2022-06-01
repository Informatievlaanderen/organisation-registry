namespace OrganisationRegistry.Infrastructure.Configuration;

using Microsoft.Data.SqlClient;

public static class Obfuscator
{
    public static string ObfuscateConnectionString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new string('*', 12);

        var builder = new SqlConnectionStringBuilder(text);
        if (!string.IsNullOrWhiteSpace(builder.Password))
            builder.Password = new string('*', 12);

        return builder.ToString();
    }
}
