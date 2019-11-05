namespace OrganisationRegistry.VlaanderenBeNotifier.UnitTests
{
    using Configuration;

    public class VlaanderenBeNotifierConfigurationTestDataBuilder
    {
        private readonly string _sendGridApiUri;
        private readonly string _sendGridBearerToken;
        private readonly string _fromAddress;
        private readonly string _fromName;
        private readonly string _organisationTo;
        private readonly string _organisationUriTemplate;
        private readonly string _bodyTo;
        private readonly string _bodyUriTemplate;

        public VlaanderenBeNotifierConfigurationTestDataBuilder()
        {
            _sendGridApiUri = "http://localhost";
            _sendGridBearerToken = "mysecretbearertoken";
            _fromAddress = "me@null.null";
            _fromName = "Me";
            _organisationTo = "you@null.null";
            _organisationUriTemplate = "http://localhost:3000/#/organisation/{0}";
            _bodyTo = "you@null.null";
            _bodyUriTemplate = "http://localhost:3000/#/bodies/{0}";
        }

        public VlaanderenBeNotifierConfiguration Build()
        {
            return new VlaanderenBeNotifierConfiguration()
            {
                SendGridApiUri = _sendGridApiUri,
                SendGridBearerToken = _sendGridBearerToken,
                FromAddress = _fromAddress,
                FromName = _fromName,
                OrganisationTo = _organisationTo,
                OrganisationUriTemplate = _organisationUriTemplate,
                BodyTo = _bodyTo,
                BodyUriTemplate = _bodyUriTemplate
            };
        }

        public static implicit operator VlaanderenBeNotifierConfiguration(VlaanderenBeNotifierConfigurationTestDataBuilder dataBuilder)
        {
            return dataBuilder.Build();
        }
    }
}
