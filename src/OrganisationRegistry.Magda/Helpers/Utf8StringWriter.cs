namespace OrganisationRegistry.Magda.Helpers
{
    using System.IO;
    using System.Text;

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
