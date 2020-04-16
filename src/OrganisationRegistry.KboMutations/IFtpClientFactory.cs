namespace OrganisationRegistry.KboMutations
{
    using System;
    using System.Net;
    using Configuration;
    using FluentFTP;
    using Microsoft.Extensions.Logging;

    public interface IFtpClientFactory
    {
        IFtpClient CreateFtpClient(
            KboMutationsConfiguration kboMutationsConfiguration,
            ILogger logger);
    }

    public class FtpClientFactory : IFtpClientFactory
    {
        public IFtpClient CreateFtpClient(
            KboMutationsConfiguration kboMutationsConfiguration,
            ILogger logger)
        {
            return new FtpClient
            {
                Host = kboMutationsConfiguration.Host,
                Port = kboMutationsConfiguration.Port,
                Credentials = new NetworkCredential(
                    kboMutationsConfiguration.Username,
                    kboMutationsConfiguration.Password),
                OnLogEvent = (level, s) =>
                {
                    switch (level)
                    {
                        case FtpTraceLevel.Verbose:
                            logger.Log(LogLevel.Trace, s);
                            break;
                        case FtpTraceLevel.Info:
                            logger.Log(LogLevel.Information, s);
                            break;
                        case FtpTraceLevel.Warn:
                            logger.Log(LogLevel.Warning, s);
                            break;
                        case FtpTraceLevel.Error:
                            logger.Log(LogLevel.Error, s);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(level), level, null);
                    }
                }
            };
        }
    }
}
