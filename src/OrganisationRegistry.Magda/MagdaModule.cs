namespace OrganisationRegistry.Magda
{
    using System.Security.Cryptography.X509Certificates;
    using Autofac;
    using Microsoft.Extensions.Configuration;

    public class MagdaModule : Module
    {
        protected internal static MagdaConfiguration MagdaConfiguration;

        public MagdaModule(IConfiguration configuration)
        {
            var apiConfiguration = configuration.GetSection("Api");

            MagdaConfiguration = new MagdaConfiguration(
                new X509Certificate2(
                    fileName: apiConfiguration["KboCertificate"],
                    password: apiConfiguration["RijksRegisterCertificatePwd"],
                    keyStorageFlags: X509KeyStorageFlags.MachineKeySet |
                                     X509KeyStorageFlags.PersistKeySet |
                                     X509KeyStorageFlags.Exportable),
                apiConfiguration.GetValue<int>("KboMagdaTimeout"),
                apiConfiguration["KboSender"],
                apiConfiguration["RijksRegisterCapacity"],
                apiConfiguration["KboRecipient"],
                apiConfiguration["KboMagdaEndpoint"]);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(MagdaConfiguration)
                .SingleInstance();
        }
    }

    public class MagdaConfiguration
    {
        public X509Certificate2 ClientCertificate { get; }
        public int Timeout { get; }

        public string Sender { get; }
        public string Capacity { get; }
        public string Recipient { get; }

        public string KBOMagdaEndPoint { get; }

        public MagdaConfiguration(X509Certificate2 clientCertificate, int timeout, string sender, string capacity, string recipient, string kboMagdaEndPoint)
        {
            ClientCertificate = clientCertificate;
            Timeout = timeout;
            Sender = sender;
            Capacity = capacity;
            Recipient = recipient;
            KBOMagdaEndPoint = kboMagdaEndPoint;
        }
    }

}
