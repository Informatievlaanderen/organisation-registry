namespace OrganisationRegistry.UI.Infrastructure
{
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder hostBuilder = new WebHostBuilder();
            var environment = hostBuilder.GetSetting("environment");

            if (environment == "Development")
            {
                var cert = new X509Certificate2("organisationregistry-ui.pfx", "organisationregistry");

                hostBuilder = hostBuilder
                    .UseKestrel(server =>
                    {
                        server.AddServerHeader = false;
                        server.Listen(IPAddress.Any, 2443, listenOptions =>
                        {
                            listenOptions.UseHttps(cert);
                        });
                    })
                    .UseUrls("https://organisatie.dev-vlaanderen.local:1443");
            }
            else
            {
                hostBuilder = hostBuilder.UseKestrel(server => server.AddServerHeader = false);
            }

            var host = hostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                //.UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
