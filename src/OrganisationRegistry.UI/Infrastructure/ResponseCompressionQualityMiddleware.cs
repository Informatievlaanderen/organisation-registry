namespace OrganisationRegistry.UI.Infrastructure;

using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Task = System.Threading.Tasks.Task;

public class ResponseCompressionQualityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDictionary<string, double> _encodingQuality;

    public ResponseCompressionQualityMiddleware(RequestDelegate next, IDictionary<string, double> encodingQuality)
    {
        _next = next;
        _encodingQuality = encodingQuality;
    }

    public async Task Invoke(HttpContext context)
    {
        var encodings = context.Request.Headers[HeaderNames.AcceptEncoding];

        if (!StringValues.IsNullOrEmpty(encodings) &&
            StringWithQualityHeaderValue.TryParseList(encodings, out var encodingsList) &&
            encodingsList.Count > 0)
        {
            var encodingsWithQuality = new string[encodingsList.Count];

            for (var encodingIndex = 0; encodingIndex < encodingsList.Count; encodingIndex++)
            {
                // If there is any quality value provided don't change anything
                if (encodingsList[encodingIndex].Quality.HasValue)
                {
                    encodingsWithQuality = null;
                    break;
                }

                var encodingValue = encodingsList[encodingIndex].Value.ToString();
                encodingsWithQuality[encodingIndex] =
                    new StringWithQualityHeaderValue(
                        encodingValue,
                        _encodingQuality.ContainsKey(encodingValue)
                            ? _encodingQuality[encodingValue]
                            : 0.1).ToString();
            }

            if (encodingsWithQuality != null)
                context.Request.Headers[HeaderNames.AcceptEncoding] = new StringValues(encodingsWithQuality);
        }

        await _next(context);
    }
}
