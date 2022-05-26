namespace OrganisationRegistry.Magda
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    public class MagdaClientCertificate : X509Certificate2
    {
        private MagdaClientCertificate(string base64Cert, string password) :
            base(
                rawData: Convert.FromBase64String(base64Cert),
                password: password,
                keyStorageFlags: X509KeyStorageFlags.MachineKeySet |
                                 X509KeyStorageFlags.PersistKeySet |
                                 X509KeyStorageFlags.Exportable)
        {
        }

        // todo: refactor: redesign calling code so this constructor is not needed anymore
        private MagdaClientCertificate()
        {
        }

        public static MagdaClientCertificate Create(string base64Cert, string password)
            => !string.IsNullOrWhiteSpace(base64Cert)
                ? new MagdaClientCertificate(base64Cert, password)
                : new MagdaClientCertificate();
    }
}
