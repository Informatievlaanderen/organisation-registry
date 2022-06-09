namespace OrganisationRegistry.Tests.Shared;

using Organisation;

public class SequentialOvoNumberGenerator : IOvoNumberGenerator
{
    private static int current;
    private static readonly object Locker = new();

    public string GenerateNumber()
    {
        lock (Locker)
            return $"OVO{++current:D6}";
    }

    public string GenerateNextNumber()
        => GenerateNumber();
}
