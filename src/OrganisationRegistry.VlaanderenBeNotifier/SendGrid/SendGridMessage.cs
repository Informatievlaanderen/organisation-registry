namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid
{
    using System.Collections.Generic;

    public class SendGridMessage
    {
        public const string TypeText = "text";
        public const string TypeHtml = "text/html";

        public List<SendGridPersonalization> Personalizations { get; set; }
        public SendGridEmail From { get; set; }
        public List<SendGridContent> Content { get; set; }
        public List<string> Categories { get; set; }

        public SendGridMessage() { }

        public SendGridMessage(IList<SendGridEmail> to, string subject, SendGridEmail from, string message, List<string> categories = null, string type = TypeHtml)
        {
            Personalizations = new List<SendGridPersonalization> { new SendGridPersonalization { To = to, Subject = subject } };
            From = from;
            Content = new List<SendGridContent> { new SendGridContent(type, message) };
            Categories = categories ?? new List<string>();
        }
    }
}
