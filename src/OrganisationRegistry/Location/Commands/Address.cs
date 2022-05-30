namespace OrganisationRegistry.Location.Commands;

public class Address
{
    public string Street { get; }
    public string ZipCode { get; }
    public string City { get; }
    public string Country { get; }

    public string FullAddress => $"{Street}, {ZipCode} {City}, {Country}";

    public Address(
        string street,
        string zipCode,
        string city,
        string country)
    {
        Street = street;
        ZipCode = zipCode;
        City = city;
        Country = country;
    }
}