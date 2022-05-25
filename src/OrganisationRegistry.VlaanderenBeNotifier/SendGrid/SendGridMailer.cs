namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class SendGridMailer : IMailer
    {
        private readonly ILogger<SendGridMailer> _logger;

        private static HttpClient _httpClient = null!;

        public SendGridMailer(
            ILogger<SendGridMailer> logger,
            IOptions<VlaanderenBeNotifierConfiguration> configurationOptions)
        {
            _logger = logger;
            var configuration = configurationOptions.Value;

            //Uri sendGridApiUri, string sendGridBearerToken

            if (configuration.SendGridApiUri == null)
                throw new ArgumentNullException(nameof(configuration.SendGridApiUri));

            if (string.IsNullOrWhiteSpace(configuration.SendGridBearerToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(configuration.SendGridBearerToken));

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration.SendGridApiUri),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", configuration.SendGridBearerToken)
                }
            };
        }

        // taken with some liberty from https://github.com/sendgrid/sendgrid-csharp/issues/221#issuecomment-230575041
        // SendGrid docs: https://sendgrid.com/docs/API_Reference/Web_API_v3/Mail/index.html
        public async Task SendEmailAsync(string to, string subject, string fromAddress, string fromName, string body, List<string> categories)
        {
            var toContacts = to
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new SendGridEmail(x))
                .ToList();

            var msg = new SendGridMessage(
                to: toContacts,
                subject: subject,
                @from: new SendGridEmail(fromAddress, fromName),
                message: body,
                categories: categories,
                type: SendGridMessage.TypeText);

            try
            {
                var json = JsonConvert.SerializeObject(msg);
                _logger.LogInformation("SendGrid Json Request: \t\n{RequestJson}", json);

                var response = await _httpClient.PostAsync("mail/send", new StringContent(json, Encoding.UTF8, "application/json"));

                _logger.LogInformation("SendGrid Response Code: \t{StatusCode}", response.StatusCode);

                // this is just a rough example of handling errors
                if (!response.IsSuccessStatusCode)
                {
                    // see if we can read the response for more information, then log the error
                    var errorJson = await response.Content.ReadAsStringAsync();
                    throw new Exception($"SendGrid indicated failure, code: {response.StatusCode}, reason: {errorJson}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Could not send mail");
                throw;
            }
        }
    }
}
