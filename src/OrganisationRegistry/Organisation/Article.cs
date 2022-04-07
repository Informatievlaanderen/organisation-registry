namespace OrganisationRegistry.Organisation
{
    using System;

    public class Article : IEquatable<Article>
    {
        public static readonly Article None = new(null);
        public static readonly Article De = new("de");
        public static readonly Article Het = new("het");

        public static readonly Article[] All = { None, De, Het };

        private readonly string? _value;

        private Article(string? value)
        {
            _value = value;
        }

        public static Article Parse(string? value)
        {
            if (string.IsNullOrEmpty(value)) return None;
            if (Array.Find(All, candidate => candidate._value == value) is { } parsed)
                return parsed;

            throw new FormatException($"The value {value} is not a well known source.");
        }

        public bool Equals(Article? other)
            => other is { } && other._value == _value;

        public override bool Equals(object? obj)
            => obj is Article type && Equals(type);

        public override int GetHashCode()
            => _value?.GetHashCode() ?? 0;

        public override string ToString()
            => _value ?? "";

        public static implicit operator string(Article instance)
            => instance.ToString();

        public static bool operator ==(Article left, Article right)
            => Equals(left, right);

        public static bool operator !=(Article left, Article right)
            => !Equals(left, right);
    }
}
