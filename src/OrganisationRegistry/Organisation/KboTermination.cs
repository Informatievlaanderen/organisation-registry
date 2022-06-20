namespace OrganisationRegistry.Organisation;

using System;

public readonly struct KboTermination
{
    public DateTime Date { get; }
    public string Code { get; }
    public string Reason { get; }

    public KboTermination(DateTime date, string code, string reason)
    {
        Date = date;
        Code = code;
        Reason = reason;
    }

    public static KboTermination FromMagda(IMagdaTermination termination)
        => new(termination.Date, termination.Code, termination.Reason);

    public bool Equals(KboTermination other)
        => Date.Equals(other.Date) && Code == other.Code && Reason == other.Reason;

    public override bool Equals(object? obj)
        => obj is KboTermination other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Date, Code, Reason);
}
