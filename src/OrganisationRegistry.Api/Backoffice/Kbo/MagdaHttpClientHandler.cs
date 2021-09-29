namespace OrganisationRegistry.Api.Backoffice.Kbo
{
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    public class MagdaHttpClientHandler : HttpClientHandler
    {
        public MagdaHttpClientHandler(X509Certificate magdaClientCertificate)
        {
            ClientCertificateOptions = ClientCertificateOption.Manual;
            SslProtocols = SslProtocols.Tls12;
            ClientCertificates.Add(magdaClientCertificate);
        }
    }
}
