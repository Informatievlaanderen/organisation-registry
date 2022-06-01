namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid;

using System.Collections.Generic;

public class SendGridPersonalization
{
    public SendGridPersonalization()
    {
        To = new List<SendGridEmail>();
        Subject = string.Empty;
    }

    public IList<SendGridEmail> To { get; set; }
    public string Subject { get; set; }
}
