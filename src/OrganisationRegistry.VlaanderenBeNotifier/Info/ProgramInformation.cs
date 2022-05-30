namespace OrganisationRegistry.VlaanderenBeNotifier.Info;

using System.Text;
using Configuration;

public class ProgramInformation
{
    public static string Build(VlaanderenBeNotifierConfiguration vlaanderenBeNotifierConfiguration)
    {
        var progInfo = new StringBuilder();
        progInfo.AppendLine();
        progInfo.AppendLine("Application settings:");
        progInfo.AppendLine(new string('-', 50));
        AppendKeyValue(progInfo, "SendGrid Bearer Token", ObfuscateToken(vlaanderenBeNotifierConfiguration.SendGridBearerToken));
        AppendKeyValue(progInfo, "FromAddress", vlaanderenBeNotifierConfiguration.FromAddress);
        AppendKeyValue(progInfo, "FromName", vlaanderenBeNotifierConfiguration.FromName);
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

    private static string ObfuscateToken(string text)
    {
        if(string.IsNullOrEmpty(text))
            return text;

        return
            text[..5] +
            new string('*', text.Length - 8) +
            text[^3..];
    }
}