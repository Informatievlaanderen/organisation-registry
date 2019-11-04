namespace OrganisationRegistry
{
    using System;

    public class GenericId<T> : IEquatable<T> where T : GenericId<T>
    {
        protected Guid Value { get; }

        public GenericId(Guid id)
            => Value = id;

        public static implicit operator Guid(GenericId<T> id)
            => id.Value;

        public static bool operator ==(GenericId<T> obj1, GenericId<T> obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return true;

            if (ReferenceEquals(obj1, null))
                return false;

            if (ReferenceEquals(obj2, null))
                return false;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(GenericId<T> obj1, GenericId<T> obj2)
            => !(obj1 == obj2);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && Equals((T) obj);
        }

        public bool Equals(T other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public override int GetHashCode()
            => Value.GetHashCode();

        public override string ToString()
            => Value.ToString();
    }
}
