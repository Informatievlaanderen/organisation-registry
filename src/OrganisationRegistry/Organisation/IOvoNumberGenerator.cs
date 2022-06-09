namespace OrganisationRegistry.Organisation;

public interface IOvoNumberGenerator
{
    string GenerateNumber();
    string GenerateNextNumber();
}
