namespace OrganisationRegistry.Api.Infrastructure.Magda;

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Used when a Magda certificate is configured but invalid.
/// Ensures the application starts up but all Magda calls fail with a clear error.
/// </summary>
public class InvalidCertificateMagdaHttpClientHandler : HttpClientHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => throw new InvalidOperationException(
            "Magda is not available: the configured KboCertificate could not be loaded. " +
            "Verify that ApiConfiguration:KboCertificate contains a valid base-64 encoded PFX certificate.");
}
