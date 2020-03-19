namespace OrganisationRegistry.KboMutations
{
    using System;
    using CsvHelper.Configuration.Attributes;

    public class MutationsLine
    {
        [Index(0)]
        public DateTime DatumModificatie { get; set; }

        [Index(1)]
        public string Ondernemingsnummer { get; set; }

        [Index(25)]
        public string MaatschappelijkeNaam { get; set; }

        protected bool Equals(MutationsLine other)
        {
            return DatumModificatie.Equals(other.DatumModificatie) &&
                   Ondernemingsnummer == other.Ondernemingsnummer &&
                   MaatschappelijkeNaam == other.MaatschappelijkeNaam;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MutationsLine) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DatumModificatie.GetHashCode();
                hashCode = (hashCode * 397) ^ (Ondernemingsnummer != null ? Ondernemingsnummer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MaatschappelijkeNaam != null ? MaatschappelijkeNaam.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
