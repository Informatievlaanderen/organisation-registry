namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid
{
    using System.Collections.Generic;

    public class SendGridPersonalization
    {
        public IList<SendGridEmail> To { get; set; }
        public string Subject { get; set; }
    }
}
