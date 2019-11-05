namespace OrganisationRegistry.Api.Infrastructure
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

            IWebHostBuilder hostBuilder = new WebHostBuilder();
            var environment = hostBuilder.GetSetting("environment");

            if (environment == "Development")
            {
                var cert = new X509Certificate2(Path.Join("..", "..", "organisationregistry-api.pfx"), "organisationregistry");

                hostBuilder = hostBuilder
                    .UseKestrel(server =>
                    {

                        server.AddServerHeader = false;

                        // Map localhost to api.wegwijs.dev.informatievlaanderen.be
                        // Then use https://api.wegwijs.dev.informatievlaanderen.be:2443/ in a browser
                        server.Listen(IPAddress.Loopback, 2443, listenOptions => listenOptions.UseConnectionLogging().UseHttps(cert));
                        server.Listen(IPAddress.Loopback, 2080, listenOptions => listenOptions.UseConnectionLogging());
                    });
            }
            else
            {
                hostBuilder = hostBuilder.UseKestrel(server => server.AddServerHeader = false);
            }

            var host = hostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
