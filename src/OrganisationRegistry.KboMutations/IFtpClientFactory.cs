namespace OrganisationRegistry.KboMutations
{
    using System.Net;
    using Configuration;
    using FluentFTP;

    public interface IFtpClientFactory
    {
        IFtpClient CreateFtpClient(KboMutationsConfiguration kboMutationsConfiguration);
    }

    public class FtpClientFactory : IFtpClientFactory
    {
        public IFtpClient CreateFtpClient(KboMutationsConfiguration kboMutationsConfiguration)
        {
            return new FtpClient
            {
                Host = kboMutationsConfiguration.Host,
                Port = kboMutationsConfiguration.Port,
                Credentials = new NetworkCredential(
                    kboMutationsConfiguration.Username,
                    kboMutationsConfiguration.Password),
            };
        }
    }
}
