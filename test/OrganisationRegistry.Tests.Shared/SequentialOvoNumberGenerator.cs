namespace OrganisationRegistry.Tests.Shared;

using Organisation;

public class SequentialOvoNumberGenerator : IOvoNumberGenerator
{
    private static int _current;
    private static readonly object Locker = new object();

    public string GenerateNumber()
    {
        lock (Locker)
            return $"OVO{++_current:D6}";
    }
}
