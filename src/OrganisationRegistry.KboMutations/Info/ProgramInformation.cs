﻿namespace OrganisationRegistry.KboMutations.Info
{
    using System.Text;
    using Api.Status;
    using Configuration;

    public class ProgramInformation
    {
        public static string Build(
            KboMutationsConfiguration kboMutationsConfiguration,
            string externalIp)
        {
            var progInfo = new StringBuilder();
            progInfo.AppendLine();
            AppendKeyValue(progInfo, "External Ip", externalIp);
            progInfo.AppendLine();
            progInfo.AppendLine("Application settings:");
            progInfo.AppendLine(new string('-', 50));
            AppendKeyValue(progInfo, "Created", ObfuscateToken(kboMutationsConfiguration.Created.ToLongTimeString()));
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
            return
                text.Substring(0, 5) +
                new string('*', text.Length - 8) +
                text.Substring(text.Length - 3);
        }
    }
}
