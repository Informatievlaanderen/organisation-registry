namespace OrganisationRegistry.UI.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Reflection;
    using Configuration.Database;
    using Configuration.Database.Configuration;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                .AddEnvironmentVariables();

            var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
            Configuration = builder
                .AddEntityFramework(x => x.UseSqlServer(
                    sqlConfiguration.ConnectionString,
                    y => y.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)))
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();

                options.EnableForHttps = true;

                options.MimeTypes = new[]
                {
                    // General
                    "text/plain",

                    // Static files
                    "text/css",
                    "application/javascript",

                    // MVC
                    "text/html",
                    "application/xml",
                    "text/xml",
                    "application/json",
                    "text/json",

                    // Fonts
                    "application/font-woff",
                    "font/otf",
                    "application/vnd.ms-fontobject"
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // TODO: redirect index.html to / (and remove from web.config)

            app.Use(async (context, next) =>
            {
                if (IsIndexPath(context))
                {
                    var parentUri = GetParentUriString(GetUri(context.Request));
                    context.Response.Redirect(parentUri, true);
                }
                else
                {
                    await next();
                }
            });

            app.UseMiddleware<AddHttpSecurityHeadersMiddleware>();
            app.UseMiddleware<AddVersionHeaderMiddleware>();

            app.UseMiddleware<ResponseCompressionQualityMiddleware>(new Dictionary<string, double>
            {
                {"br", 1.0},
                {"gzip", 0.9}
            });
            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.Run(async context =>
            {
                if (IsConfigPath(context))
                {
                    context.Response.ContentType = "application/javascript";
                    await context.Response.WriteAsync(BuildConfigJavascript(env));
                }
            });
        }

        private static bool IsIndexPath(HttpContext context)
        {
            return context.Request.Path.HasValue &&
                   context.Request.Path.Value.ToLowerInvariant().Contains("/index.html");
        }

        private static Uri GetUri(HttpRequest request)
        {
            var builder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            };

            if (request.Host.Port.HasValue)
                builder.Port = request.Host.Port.Value;

            return builder.Uri;
        }

        private static string GetParentUriString(Uri uri)
        {
            return uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length);
        }

        private static bool IsConfigPath(HttpContext context)
        {
            return context.Request.Path.HasValue &&
                   string.Equals(context.Request.Path.Value.ToLowerInvariant(), "/config.js", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildConfigJavascript(IHostingEnvironment env)
        {
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            return $"window.organisationRegistryVersion = '{version}';\n" +
                   $"window.organisationRegistryApiEndpoint = '{Configuration["Api:Endpoint"]}';\n" +
                   $"window.organisationRegistryEnvironment = '{env.EnvironmentName}';\n" +
                   $"window.organisationRegistryUiEndpoint = '{Configuration["UI:Endpoint"]}';\n" +
                   $"window.organisationRegistryAuthEndpoint = '{Configuration["Auth:Endpoint"]}';\n" +
                   $"window.organisationRegistryInformatieVlaanderenLink = '{Configuration["UI:InformatieVlaanderenLink"]}';";
        }
    }
}
