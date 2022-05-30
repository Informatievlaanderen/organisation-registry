namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMailer
{
    Task SendEmailAsync(string to, string subject, string fromAddress, string fromName, string body, List<string> categories);
}