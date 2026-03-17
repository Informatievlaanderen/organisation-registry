# Location Registratie en Creatie

## Overzicht

Dit document beschrijft hoe Location entities worden aangemaakt en geregistreerd in het systeem. Een Location is een fysieke adres entiteit die onafhankelijk bestaat en vervolgens gekoppeld kan worden aan organisaties via OrganisationLocation relaties.

## Architectuur

### Event Sourcing Pattern

Location creatie volgt het event sourcing pattern:

```
API Request → Command → Command Handler → Aggregate → Event → Projection
```

1. **API Request**: HTTP POST naar `/locations` met adresgegevens
2. **Command**: `CreateLocation` command wordt aangemaakt
3. **Command Handler**: `LocationCommandHandlers.Handle()` valideert en verwerkt
4. **Aggregate**: `Location` aggregate emits `LocationCreated` event
5. **Event**: `LocationCreated` wordt opgeslagen in event store
6. **Projection**: `LocationListView` projecteert event naar read database

## Location Entity

### Aggregate Root

**Bestand**: `src/OrganisationRegistry/Location/Location.cs`

```csharp
public class Location : AggregateRoot
{
    private string? _crabLocationId;
    private string _street;
    private string _zipCode;
    private string _city;
    private string _country;
    public string FormattedAddress { get; private set; }

    public Location(LocationId id, string? crabLocationId, Address address)
    {
        ApplyChange(
            new LocationCreated(
                id,
                crabLocationId,
                address.FullAddress,
                address.Street,
                address.ZipCode,
                address.City,
                address.Country));
    }
}
```

### Kenmerken

- **LocationId**: Guid, unieke identifier
- **CrabLocationId**: Optioneel, string voor CRAB (Centraal Referentie Adressenbestand) integratie
- **FormattedAddress**: Volledige adres string `"{Street}, {ZipCode} {City}, {Country}"`
- **Street**: Straatnaam met huisnummer
- **ZipCode**: Postcode
- **City**: Gemeente/stad
- **Country**: Land

## API Endpoints

### POST /locations - Location Aanmaken

**Bestand**: `src/OrganisationRegistry.Api/Backoffice/Parameters/Location/LocationCommandController.cs:29`

```http
POST /v1/locations
Authorization: Bearer <token>
Content-Type: application/json

{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "crabLocationId": "12345",  // Optioneel
  "street": "Kunstlaan 16",
  "zipCode": "1000",
  "city": "Brussel",
  "country": "België"
}
```

**Response**:
- `201 Created` met Location header naar `/locations/{id}`
- `400 Bad Request` bij validatiefouten
- `403 Forbidden` bij onvoldoende rechten

### GET /locations - Locations Oplijsten

```http
GET /v1/locations?filter[street]=Kunstlaan&sortBy=city&sortOrder=asc&offset=0&limit=20
```

**Response**: Gepagineerde lijst van locations

### GET /locations/{id} - Location Ophalen

```http
GET /v1/locations/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**Response**:
- `200 OK` met location details
- `404 Not Found` als location niet bestaat

### PUT /locations/{id} - Location Updaten

**Bestand**: `src/OrganisationRegistry.Api/Backoffice/Parameters/Location/LocationCommandController.cs:46`

```http
PUT /v1/locations/3fa85f64-5717-4562-b3fc-2c963f66afa6
Authorization: Bearer <token>
Content-Type: application/json

{
  "crabLocationId": "12345",
  "street": "Kunstlaan 17",
  "zipCode": "1000",
  "city": "Brussel",
  "country": "België"
}
```

## Commands

### CreateLocation Command

**Bestand**: `src/OrganisationRegistry/Location/Commands/CreateLocation.cs`

```csharp
public class CreateLocation : BaseCommand<LocationId>
{
    public LocationId LocationId { get; }
    public string? CrabLocationId { get; }
    public Address Address { get; }

    public CreateLocation(
        LocationId locationId,
        string? crabLocationId,
        string street,
        string zipCode,
        string city,
        string country)
}
```

### Address Value Object

**Bestand**: `src/OrganisationRegistry/Location/Commands/Address.cs`

```csharp
public class Address
{
    public string Street { get; }
    public string ZipCode { get; }
    public string City { get; }
    public string Country { get; }
    public string FullAddress => $"{Street}, {ZipCode} {City}, {Country}";
}
```

## Validatie

### FluentValidation Rules

**Bestand**: `src/OrganisationRegistry.Api/Backoffice/Parameters/Location/Requests/CreateLocationRequest.cs:21`

```csharp
public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
{
    public CreateLocationRequestValidator()
    {
        // ID validatie
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        // Street validatie
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required.");

        RuleFor(x => x.Street)
            .Length(0, 200)  // LocationListConfiguration.StreetLength
            .WithMessage("Street cannot be longer than 200.");

        // ZipCode validatie
        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip Code is required.");

        RuleFor(x => x.ZipCode)
            .Length(0, 50)  // LocationListConfiguration.ZipCodeLength
            .WithMessage("Zip Code cannot be longer than 50.");

        // City validatie
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.");

        RuleFor(x => x.City)
            .Length(0, 100)  // LocationListConfiguration.CityLength
            .WithMessage("City cannot be longer than 100.");

        // Country validatie
        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.");

        RuleFor(x => x.Country)
            .Length(0, 100)  // LocationListConfiguration.CountryLength
            .WithMessage("Country cannot be longer than 100.");
    }
}
```

### Field Length Constraints

**Bestand**: `src/OrganisationRegistry.SqlServer/Location/LocationListView.cs:30-34`

```csharp
public const int CityLength = 100;
public const int ZipCodeLength = 50;
public const int StreetLength = 200;
public const int CountryLength = 100;
public const int FormattedAddressLength = 460; // Sum + 10 for spaces and commas
```

### Unique Name Validation

**Bestand**: `src/OrganisationRegistry.Api/Backoffice/Parameters/Location/UniqueNameValidator.cs`

De `FormattedAddress` moet uniek zijn in het systeem:

```csharp
public class UniqueNameValidator : IUniqueNameValidator<Location>
{
    public bool IsNameTaken(string name)
    {
        return _context.LocationList.Any(item => item.FormattedAddress == name);
    }

    public bool IsNameTaken(Guid id, string name)
    {
        return _context.LocationList
            .Where(item => item.Id != id)
            .Any(item => item.FormattedAddress == name);
    }
}
```

**Exception bij duplicaat**: `NameNotUnique` wordt gegooid als FormattedAddress al bestaat.

## Authorization

### Required Roles

**Bestand**: `src/OrganisationRegistry/Location/LocationCommandHandlers.cs:29`

Alleen gebruikers met één van deze rollen kunnen locations aanmaken of wijzigen:

- **AlgemeenBeheerder**: Algemene beheerder met volledige rechten
- **CjmBeheerder**: CJM (Contactpunt Jaarlijkse Meetstaat) beheerder

```csharp
public async Task Handle(ICommandEnvelope<CreateLocation> envelope)
    => await Handler.For(envelope.User, Session)
        .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
        .Handle(/* ... */);
```

## Command Handler

### CreateLocation Handler

**Bestand**: `src/OrganisationRegistry/Location/LocationCommandHandlers.cs:27-38`

```csharp
public async Task Handle(ICommandEnvelope<CreateLocation> envelope)
    => await Handler.For(envelope.User, Session)
        .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
        .Handle(
            session =>
            {
                // 1. Valideer uniciteit FormattedAddress
                if (_uniqueNameValidator.IsNameTaken(envelope.Command.Address.FullAddress))
                    throw new NameNotUnique();

                // 2. Maak Location aggregate aan (emits LocationCreated event)
                var location = new Location(
                    envelope.Command.LocationId,
                    envelope.Command.CrabLocationId,
                    envelope.Command.Address);

                // 3. Voeg toe aan session (event sourcing)
                session.Add(location);
            });
```

### Process Flow

1. **Authorization Check**: Controleer of gebruiker AlgemeenBeheerder of CjmBeheerder is
2. **Unique Name Validation**: Controleer of FormattedAddress niet al bestaat
3. **Create Aggregate**: Nieuwe Location aggregate wordt aangemaakt
4. **Emit Event**: Location constructor emits `LocationCreated` event via `ApplyChange()`
5. **Session Add**: Aggregate wordt toegevoegd aan event sourcing session
6. **Commit**: Session wordt gecommit, event wordt opgeslagen in event store
7. **Event Processing**: Event handlers verwerken `LocationCreated` event asynchroon

## Events

### LocationCreated Event

**Bestand**: `src/OrganisationRegistry/Location/Events/LocationCreated.cs`

```csharp
public class LocationCreated : BaseEvent<LocationCreated>, IHasLocation
{
    public Guid LocationId { get; }
    public string? CrabLocationId { get; set; }
    public string FormattedAddress { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public LocationCreated(
        Guid locationId,
        string? crabLocationId,
        string formattedAddress,
        string street,
        string zipCode,
        string city,
        string country)
}
```

### LocationUpdated Event

**Bestand**: `src/OrganisationRegistry/Location/Events/LocationUpdated.cs`

Bij updates worden zowel nieuwe als oude waarden opgeslagen voor audit trail:

```csharp
public class LocationUpdated : BaseEvent<LocationUpdated>
{
    public Guid LocationId { get; }

    // Nieuwe waarden
    public string? CrabLocationId { get; set; }
    public string FormattedAddress { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    // Vorige waarden (voor audit trail)
    public string? PreviousCrabLocationId { get; set; }
    public string PreviousFormattedAddress { get; set; }
    public string PreviousStreet { get; set; }
    public string PreviousZipCode { get; set; }
    public string PreviousCity { get; set; }
    public string PreviousCountry { get; set; }
}
```

## Database Projectie

### LocationListView Projection

**Bestand**: `src/OrganisationRegistry.SqlServer/Location/LocationListView.cs:97-116`

De `LocationListView` luistert naar `LocationCreated` events en projecteert deze naar de read database:

```csharp
public async Task Handle(
    DbConnection dbConnection,
    DbTransaction dbTransaction,
    IEnvelope<LocationCreated> message)
{
    var location = new LocationListItem
    {
        Id = message.Body.LocationId,
        CrabLocationId = message.Body.CrabLocationId,
        FormattedAddress = message.Body.FormattedAddress,
        Street = message.Body.Street,
        ZipCode = message.Body.ZipCode,
        City = message.Body.City,
        Country = message.Body.Country,
        HasCrabLocation = !string.IsNullOrWhiteSpace(message.Body.CrabLocationId),
    };

    using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
    {
        await context.LocationList.AddAsync(location);
        await context.SaveChangesAsync();
    }
}
```

### LocationListItem Entity

**Bestand**: `src/OrganisationRegistry.SqlServer/Location/LocationListView.cs:15-26`

```csharp
public class LocationListItem
{
    public Guid Id { get; set; }
    public string? CrabLocationId { get; set; }
    public string? FormattedAddress { get; set; }
    public string Street { get; set; }
    public string ZipCode { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public bool HasCrabLocation { get; set; }  // Computed: true if CrabLocationId is set
}
```

### Database Schema

**Tabel**: `Backoffice.LocationList`

**Bestand**: `src/OrganisationRegistry.SqlServer/Location/LocationListView.cs:36-71`

```sql
CREATE TABLE [Backoffice].[LocationList] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY NONCLUSTERED,
    [CrabLocationId] NVARCHAR(MAX) NULL,
    [FormattedAddress] NVARCHAR(460) NULL,
    [Street] NVARCHAR(200) NOT NULL,
    [ZipCode] NVARCHAR(50) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    [HasCrabLocation] BIT NOT NULL
);

-- Indexes
CREATE CLUSTERED INDEX IX_LocationList_FormattedAddress
    ON [Backoffice].[LocationList] ([FormattedAddress]);

CREATE INDEX IX_LocationList_Street
    ON [Backoffice].[LocationList] ([Street]);

CREATE INDEX IX_LocationList_ZipCode
    ON [Backoffice].[LocationList] ([ZipCode]);

CREATE INDEX IX_LocationList_City
    ON [Backoffice].[LocationList] ([City]);

CREATE INDEX IX_LocationList_Country
    ON [Backoffice].[LocationList] ([Country]);

CREATE INDEX IX_LocationList_HasCrabLocation
    ON [Backoffice].[LocationList] ([HasCrabLocation]);
```

**Index Strategy**:
- **Clustered Index** op `FormattedAddress`: Voor unieke adres lookups (gebruikt door `UniqueNameValidator`)
- **Nonclustered Indexes** op individuele adresvelden: Voor filtering en zoeken per veld

## CRAB Integratie

### Wat is CRAB?

**CRAB** = **C**entraal **R**eferentie **A**dressen**b**estand

CRAB is het Vlaamse adressenregister dat officiële adresgegevens bijhoudt. Het veld `CrabLocationId` is optioneel en wordt gebruikt wanneer een location gekoppeld is aan een officieel CRAB adres.

### HasCrabLocation Flag

De `HasCrabLocation` boolean geeft aan of een location gekoppeld is aan CRAB:

```csharp
HasCrabLocation = !string.IsNullOrWhiteSpace(message.Body.CrabLocationId)
```

Dit kan gebruikt worden voor:
- Filtering van locations met/zonder CRAB koppeling
- Validatie of officiële adressen worden gebruikt
- Reporting over data kwaliteit

**Let op**: De CRAB integratie lijkt legacy te zijn - er is geen actieve code die automatisch CRAB data synchroniseert. Het veld wordt handmatig ingevuld bij creation/update.

## Update Flow

### UpdateLocation Command

**Bestand**: `src/OrganisationRegistry/Location/Commands/UpdateLocation.cs`

Locations kunnen geüpdatet worden, waarbij alle adresvelden kunnen wijzigen:

```csharp
PUT /v1/locations/{id}
{
  "crabLocationId": "67890",
  "street": "Wetstraat 16",
  "zipCode": "1040",
  "city": "Brussel",
  "country": "België"
}
```

### Update Handler

**Bestand**: `src/OrganisationRegistry/Location/LocationCommandHandlers.cs:40-51`

```csharp
public async Task Handle(ICommandEnvelope<UpdateLocation> envelope)
    => await UpdateHandler<Location>.For(envelope.Command, envelope.User, Session)
        .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
        .Handle(
            session =>
            {
                // 1. Valideer uniciteit FormattedAddress (exclusief huidige location)
                if (_uniqueNameValidator.IsNameTaken(
                    envelope.Command.LocationId,
                    envelope.Command.Address.FullAddress))
                    throw new NameNotUnique();

                // 2. Haal location op en update
                var location = session.Get<Location>(envelope.Command.LocationId);
                location.Update(envelope.Command.CrabLocationId, envelope.Command.Address);
            });
```

### Update in Aggregate

**Bestand**: `src/OrganisationRegistry/Location/Location.cs:47-58`

```csharp
public void Update(string? crabLocationId, Address address)
{
    ApplyChange(new LocationUpdated(
        Id,
        crabLocationId,
        address.FullAddress,
        address.Street,
        address.ZipCode,
        address.City,
        address.Country,
        _crabLocationId,           // Previous values
        FormattedAddress,
        _street,
        _zipCode,
        _city,
        _country));
}
```

## Error Handling

### Mogelijke Exceptions

1. **NameNotUnique**:
   - Thrown by: `UniqueNameValidator`
   - Wanneer: FormattedAddress bestaat al
   - HTTP Status: 400 Bad Request

2. **ValidationException** (FluentValidation):
   - Thrown by: FluentValidation validators
   - Wanneer: Veldvalidatie faalt (required, length, etc.)
   - HTTP Status: 400 Bad Request

3. **UnauthorizedException**:
   - Thrown by: Authorization policy
   - Wanneer: Gebruiker heeft geen AlgemeenBeheerder of CjmBeheerder rol
   - HTTP Status: 403 Forbidden

4. **AggregateNotFoundException**:
   - Thrown by: Session.Get<Location>()
   - Wanneer: Location niet gevonden bij update
   - HTTP Status: 404 Not Found

### Error Response Format

```json
{
  "message": "Street is required.",
  "errors": [
    {
      "field": "street",
      "message": "Street is required."
    }
  ]
}
```

## Sequence Diagram - Location Creation

```
User                API                     CommandHandler          Location            EventStore          Projection
 |                   |                           |                      |                   |                   |
 |-- POST /locations-|                           |                      |                   |                   |
 |                   |-- CreateLocation cmd ---->|                      |                   |                   |
 |                   |                           |                      |                   |                   |
 |                   |                           |-- Check Role ------->|                   |                   |
 |                   |                           |<-- OK ---------------|                   |                   |
 |                   |                           |                      |                   |                   |
 |                   |                           |-- Validate Unique -->|                   |                   |
 |                   |                           |<-- OK ---------------|                   |                   |
 |                   |                           |                      |                   |                   |
 |                   |                           |-- new Location() --->|                   |                   |
 |                   |                           |                      |-- LocationCreated->|                   |
 |                   |                           |                      |   (event)         |                   |
 |                   |                           |                      |                   |                   |
 |                   |                           |-- Session.Commit() ------------------------>|                   |
 |                   |                           |                      |                   |-- Store Event --->|
 |                   |                           |                      |                   |                   |
 |                   |                           |                      |                   |-- Notify -------->|
 |                   |                           |                      |                   |                   |
 |                   |                           |                      |                   |                   |<-- Handle Event --|
 |                   |                           |                      |                   |                   |-- Insert LocationListItem
 |                   |                           |                      |                   |                   |
 |<-- 201 Created ---|                           |                      |                   |                   |
 |    Location: /locations/{id}                 |                      |                   |                   |
```

## Belangrijke Design Patterns

### 1. Event Sourcing
Alle state changes worden opgeslagen als events in de event store. De huidige state kan worden gereconstrueerd door alle events opnieuw af te spelen.

### 2. CQRS (Command Query Responsibility Segregation)
- **Write Side**: Commands worden verwerkt via aggregates, emitting events
- **Read Side**: Events worden geprojecteerd naar gedenormaliseerde read models (LocationList)

### 3. Domain-Driven Design
- **Aggregate Root**: Location is een aggregate root die zijn eigen consistentie garandeert
- **Value Object**: Address is een value object (immutable, geen identity)
- **Repository Pattern**: ISession abstracteert de persistence layer

### 4. Validation Layers
1. **API Layer**: FluentValidation voor input validatie
2. **Command Handler**: Business rule validatie (unieke naam)
3. **Aggregate**: Domain invariants (via events)

## Best Practices

### 1. Gebruik Formatted Address voor Deduplicatie

Bij het aanmaken van locations, check altijd eerst of het adres al bestaat:

```csharp
// ❌ Fout: Directe database check
var exists = context.LocationList.Any(l =>
    l.Street == street && l.City == city && l.ZipCode == zipCode);

// ✅ Correct: Gebruik UniqueNameValidator met FormattedAddress
if (_uniqueNameValidator.IsNameTaken(address.FullAddress))
    throw new NameNotUnique();
```

### 2. Altijd Zelfde Formatting

Gebruik de `Address.FullAddress` property voor consistente formatting:

```csharp
// ❌ Fout: Custom formatting
var formatted = $"{street} {zipCode} {city} {country}";

// ✅ Correct: Use Address value object
var address = new Address(street, zipCode, city, country);
var formatted = address.FullAddress; // "{Street}, {ZipCode} {City}, {Country}"
```

### 3. CRAB ID is Optioneel

Maak geen aannames over de aanwezigheid van CRAB IDs:

```csharp
// ✅ Correct: Treat CrabLocationId as optional
var location = new Location(id, crabLocationId: null, address);

// ✅ Correct: Check HasCrabLocation flag
if (location.HasCrabLocation)
{
    // Process CRAB-specific logic
}
```

### 4. Update via Commands

Wijzig nooit direct de projection (LocationList), altijd via commands:

```csharp
// ❌ Fout: Direct update projection
locationListItem.Street = "Nieuwe straat";
context.SaveChanges();

// ✅ Correct: Send UpdateLocation command
await commandSender.Send(new UpdateLocation(id, crabId, newAddress));
```

## Testing

### Unit Tests

Test de Location aggregate:

```csharp
[Fact]
public void LocationCreated_Event_Is_Applied_Correctly()
{
    // Arrange
    var id = Guid.NewGuid();
    var address = new Address("Kunstlaan 16", "1000", "Brussel", "België");

    // Act
    var location = new Location(new LocationId(id), null, address);

    // Assert
    Assert.Equal(id, location.Id);
    Assert.Equal("Kunstlaan 16, 1000 Brussel, België", location.FormattedAddress);
}
```

### Integration Tests

Test de volledige flow:

```csharp
[Fact]
public async Task CreateLocation_Should_Create_Location_In_Database()
{
    // Arrange
    var command = new CreateLocation(
        new LocationId(Guid.NewGuid()),
        null,
        "Kunstlaan 16",
        "1000",
        "Brussel",
        "België");

    // Act
    await commandSender.Send(command);

    // Assert
    var location = await context.LocationList
        .FirstOrDefaultAsync(l => l.Id == command.LocationId);

    Assert.NotNull(location);
    Assert.Equal("Kunstlaan 16", location.Street);
    Assert.Equal("Kunstlaan 16, 1000 Brussel, België", location.FormattedAddress);
}
```

### Validation Tests

Test de validators:

```csharp
[Fact]
public void CreateLocationRequest_Should_Fail_When_Street_Is_Empty()
{
    // Arrange
    var request = new CreateLocationRequest
    {
        Id = Guid.NewGuid(),
        Street = "",  // Invalid
        ZipCode = "1000",
        City = "Brussel",
        Country = "België"
    };

    var validator = new CreateLocationRequestValidator();

    // Act
    var result = validator.Validate(request);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == "Street");
}
```

## Configuratie

### Required Configuration

In `appsettings.json` zijn geen specifieke location-gerelateerde settings vereist. Wel nodig:

1. **Database Connection String**: Voor OrganisationRegistryContext
2. **Event Store Configuration**: Voor event persistence
3. **Authorization Settings**: Voor role-based access control

### Dependency Injection Setup

Registreer de benodigde services in Startup.cs:

```csharp
services.AddScoped<IUniqueNameValidator<Location>, UniqueNameValidator>();
services.AddScoped<LocationCommandHandlers>();
services.AddProjection<LocationListView>();
```

## Veelvoorkomende Scenario's

### Scenario 1: Nieuwe Location Aanmaken

```csharp
var command = new CreateLocation(
    new LocationId(Guid.NewGuid()),
    crabLocationId: null,
    street: "Wetstraat 16",
    zipCode: "1040",
    city: "Etterbeek",
    country: "België"
);

await commandSender.Send(command);
```

### Scenario 2: Location met CRAB ID

```csharp
var command = new CreateLocation(
    new LocationId(Guid.NewGuid()),
    crabLocationId: "12345",  // CRAB referentie
    street: "Kunstlaan 10-11",
    zipCode: "1210",
    city: "Sint-Joost-ten-Node",
    country: "België"
);

await commandSender.Send(command);
```

### Scenario 3: Adres Wijzigen

```csharp
var command = new UpdateLocation(
    locationId: existingLocationId,
    crabLocationId: "67890",
    street: "Wetstraat 18",  // Huisnummer gewijzigd
    zipCode: "1040",
    city: "Etterbeek",
    country: "België"
);

await commandSender.Send(command);
```

### Scenario 4: Duplicaat Preventie

```csharp
// Eerste location
var location1 = new CreateLocation(
    new LocationId(Guid.NewGuid()),
    null,
    "Kunstlaan 16",
    "1000",
    "Brussel",
    "België"
);
await commandSender.Send(location1);  // ✅ OK

// Poging tot duplicaat
var location2 = new CreateLocation(
    new LocationId(Guid.NewGuid()),
    null,
    "Kunstlaan 16",  // Zelfde adres
    "1000",
    "Brussel",
    "België"
);
await commandSender.Send(location2);  // ❌ Throws NameNotUnique
```

## Relatie met Organisaties

Nadat een Location is aangemaakt, kan deze gekoppeld worden aan organisaties via `OrganisationLocation`:

```csharp
// 1. Maak location aan
var locationId = Guid.NewGuid();
await commandSender.Send(new CreateLocation(
    new LocationId(locationId),
    null,
    "Kunstlaan 16",
    "1000",
    "Brussel",
    "België"
));

// 2. Koppel aan organisatie
await commandSender.Send(new AddOrganisationLocation(
    organisationId: myOrgId,
    organisationLocationId: Guid.NewGuid(),
    locationId: locationId,
    isMainLocation: true,
    locationTypeId: registeredOfficeTypeId,
    validFrom: DateTime.UtcNow,
    validTo: null
));
```

Zie `LOCATIE_INTEGRATIE_EXTERNE_SYSTEMEN.md` voor details over de koppeling tussen organisaties en locations.

## Conclusie

De Location registratie volgt een clean architecture met duidelijke scheiding tussen:

1. **API Layer**: HTTP endpoints en request/response DTOs
2. **Application Layer**: Command handlers en validatie
3. **Domain Layer**: Aggregates, events en business logic
4. **Infrastructure Layer**: Database projecties en persistence

Door event sourcing is een volledige audit trail beschikbaar van alle location changes. De uniciteit van adressen wordt gegarandeerd via FormattedAddress matching. Authorization zorgt ervoor dat alleen beheerders locations kunnen aanmaken of wijzigen.
