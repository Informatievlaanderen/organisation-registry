namespace OrganisationRegistry.VlaanderenBeNotifier
{
    public class Mail
    {
        public string Body { get; }
        public string Subject { get; }

        public Mail(string subject, string body)
        {
            Body = body;
            Subject = subject;
        }
    }
}
