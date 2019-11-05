namespace OrganisationRegistry.VlaanderenBeNotifier.SendGrid
{
    public class SendGridContent
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public SendGridContent() { }

        public SendGridContent(string type, string content)
        {
            Type = type;
            Value = content;
        }
    }
}
