namespace OrganisationRegistry.Tests.Shared;

using Body;

public class SequentialBodyNumberGenerator : IBodyNumberGenerator
{
    private static int _current;
    private static readonly object Locker = new();

    public string GenerateNumber()
    {
        lock (Locker)
            return $"ORG{++_current:D6}";
    }
}
