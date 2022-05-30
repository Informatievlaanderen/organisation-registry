namespace OrganisationRegistry.Magda
{
    using Autofac;
    using Microsoft.Extensions.Configuration;

    public class MagdaModule : Module
    {
        protected internal readonly MagdaConfiguration MagdaConfiguration;

        public MagdaModule(IConfiguration configuration)
        {
            var apiConfiguration = configuration.GetSection("Api");

            var certificate = GetMaybeMagdaClientCertificate(apiConfiguration["KboCertificate"], apiConfiguration["RijksRegisterCertificatePwd"]);

            MagdaConfiguration = new MagdaConfiguration(
                certificate,
                apiConfiguration.GetValue<int>("KboMagdaTimeout"),
                apiConfiguration["KboSender"],
                apiConfiguration["RijksRegisterCapacity"],
                apiConfiguration["KboRecipient"],
                apiConfiguration["KboMagdaEndpoint"],
                apiConfiguration["RepertoriumMagdaEndpoint"],
                apiConfiguration["RepertoriumCapacity"]);
        }

        private static MagdaClientCertificate? GetMaybeMagdaClientCertificate(string? maybeCertificateString, string pwd)
            => maybeCertificateString is { } certificateString && !string.IsNullOrWhiteSpace(certificateString)
                ? MagdaClientCertificate.Create(certificateString, pwd)
                : null;

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(MagdaConfiguration)
                .SingleInstance();
        }

        public const string HttpClientName = "MagdaClient";
    }

    public class MagdaConfiguration
    {
        public MagdaClientCertificate? ClientCertificate { get; }
        public int Timeout { get; }

        public string Sender { get; }
        public string Capacity { get; }
        public string Recipient { get; }

        public string KBOMagdaEndPoint { get; }
        public string RepertoriumMagdaEndpoint { get; set; }
        public string RepertoriumCapacity { get; set; }

        public MagdaConfiguration(
            MagdaClientCertificate? clientCertificate,
            int timeout,
            string sender,
            string capacity,
            string recipient,
            string kboMagdaEndPoint,
            string repertoriumMagdaEndpoint,
            string repertoriumCapacity)
        {
            ClientCertificate = clientCertificate;
            Timeout = timeout;
            Sender = sender;
            Capacity = capacity;
            Recipient = recipient;
            KBOMagdaEndPoint = kboMagdaEndPoint;
            RepertoriumMagdaEndpoint = repertoriumMagdaEndpoint;
            RepertoriumCapacity = repertoriumCapacity;
        }
    }
}
