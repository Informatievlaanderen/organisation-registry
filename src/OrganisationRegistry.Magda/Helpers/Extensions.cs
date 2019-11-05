namespace OrganisationRegistry.Magda.Helpers
{
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        public static string ToDigitsOnly(this string input)
        {
            var digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(input, "");
        }

        public static string ToKboDotFormat(this string kboNumber)
        {
            kboNumber = kboNumber.ToDigitsOnly();

            if (kboNumber.Length == 10)
            {
                var sb = new StringBuilder(kboNumber);
                sb.Insert(4, '.');
                sb.Insert(8, '.');
                kboNumber = sb.ToString();
            }

            return kboNumber;
        }
    }
}
