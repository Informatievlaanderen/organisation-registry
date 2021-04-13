namespace OrganisationRegistry.Organisation
{
    using System.Text;
    using System.Text.RegularExpressions;

    public class KboNumber
    {
        private readonly string _value;

        public KboNumber(string value)
        {
            _value = value;
        }

        public string ToDigitsOnly()
        {
            var digitsOnlyRegex = new Regex(@"[^\d]");
            return digitsOnlyRegex.Replace(_value, "");
        }

        public string ToDotFormat()
        {
            var digitsOnly = ToDigitsOnly();

            if (digitsOnly.Length == 10)
            {
                var sb = new StringBuilder(digitsOnly);
                sb.Insert(4, '.');
                sb.Insert(8, '.');
                digitsOnly = sb.ToString();
            }

            return digitsOnly;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
