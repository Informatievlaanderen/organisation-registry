namespace OrganisationRegistry.Organisation
{
    using System;

    public class Article : IEquatable<Article>
    {
        public static readonly Article None =
            new Article(
                null
            );
        public static readonly Article De =
            new Article(
                "de"
            );
        public static readonly Article Het =
            new Article(
                "het");

        public static readonly Article[] All = {None, De, Het};

        private readonly string? _value;

        private Article(string? value)
        {
            _value = value;
        }

        public static bool CanParse(string value)
        {
            return Array.Find(All, candidate => candidate._value == value) != null;
        }

        public static bool TryParse(string value, out Article parsed)
        {
            parsed = Array.Find(All, candidate => candidate._value == value);
            return parsed != null;
        }

        public static Article Parse(string? value)
        {
            if (string.IsNullOrEmpty(value)) return None;
            if (!TryParse(value, out var parsed))
            {
                throw new FormatException($"The value {value} is not a well known article.");
            }
            return parsed;
        }

        public bool Equals(Article? other) => other! != null! && other._value == _value;
        public override bool Equals(object? obj) => obj is Article type && Equals(type);
        public override int GetHashCode() => _value.GetHashCode();
        public override string ToString() => _value;
        public static implicit operator string(Article instance) => instance.ToString();
        public static bool operator ==(Article left, Article right) => Equals(left, right);
        public static bool operator !=(Article left, Article right) => !Equals(left, right);
    }
}
