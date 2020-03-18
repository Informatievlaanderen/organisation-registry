namespace OrganisationRegistry.Magda
{
    using Autofac;
    using Microsoft.Extensions.Configuration;

    public class MagdaModule : Module
    {
        protected internal static MagdaConfiguration MagdaConfiguration;

        public MagdaModule(IConfiguration configuration)
        {
            var apiConfiguration = configuration.GetSection("Api");

            var certificate = MagdaClientCertificate.Create(
                apiConfiguration["KboCertificate"],
                apiConfiguration["RijksRegisterCertificatePwd"]);

            MagdaConfiguration = new MagdaConfiguration(
                certificate,
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

        public const string HttpClientName = "MagdaClient";
    }

    public class MagdaConfiguration
    {
        public MagdaClientCertificate ClientClientCertificate { get; }
        public int Timeout { get; }

        public string Sender { get; }
        public string Capacity { get; }
        public string Recipient { get; }

        public string KBOMagdaEndPoint { get; }

        public MagdaConfiguration(MagdaClientCertificate clientClientCertificate, int timeout, string sender, string capacity, string recipient, string kboMagdaEndPoint)
        {
            ClientClientCertificate = clientClientCertificate;
            Timeout = timeout;
            Sender = sender;
            Capacity = capacity;
            Recipient = recipient;
            KBOMagdaEndPoint = kboMagdaEndPoint;
        }
    }
}
