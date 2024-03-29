﻿ namespace OrganisationRegistry.KboMutations.Info;

 using System.Text;
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
