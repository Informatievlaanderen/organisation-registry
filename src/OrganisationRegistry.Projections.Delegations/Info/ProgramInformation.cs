namespace OrganisationRegistry.Projections.Delegations.Info;

using System.Text;
using Configuration;
using OrganisationRegistry.Infrastructure.Configuration;

public class ProgramInformation
{
    public static string Build(DelegationsRunnerConfiguration delegationsRunnerConfiguration, TogglesConfigurationSection togglesConfiguration)
    {
        var progInfo = new StringBuilder();
        progInfo.AppendLine();
        progInfo.AppendLine("Application settings:");
        progInfo.AppendLine(new string('-', 50));
        AppendKeyValue(progInfo, "ApiAvailable", togglesConfiguration.ApiAvailable.ToString());
        AppendKeyValue(progInfo, "DelegationsRunnerAvailable", togglesConfiguration.DelegationsRunnerAvailable.ToString());
        progInfo.AppendLine(new string('-', 50));
        AppendKeyValue(progInfo, "LockRegionEndPoint", delegationsRunnerConfiguration.LockRegionEndPoint);
        AppendKeyValue(progInfo, "LockTableName", delegationsRunnerConfiguration.LockTableName);
        AppendKeyValue(progInfo, "LockLeasePeriodInMinutes", delegationsRunnerConfiguration.LockLeasePeriodInMinutes.ToString());
        progInfo.AppendLine(new string('-', 50));
        progInfo.AppendLine();
        return progInfo.ToString();
    }

    private static void AppendKeyValue(StringBuilder progInfo, string key, string value)
    {
        progInfo.Append(key);
        progInfo.Append(": \t");
        progInfo.AppendLine(value);
    }
}