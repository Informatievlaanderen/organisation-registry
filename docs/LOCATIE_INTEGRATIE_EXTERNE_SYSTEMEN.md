# Locatie Integratie met Externe Systemen

**Datum**: 25 februari 2026  
**Versie**: 1.0

---

## Inhoudsopgave

1. [Overzicht](#1-overzicht)
2. [Locatie Bronnen (Sources)](#2-locatie-bronnen-sources)
3. [Externe Systeem: KBO via MAGDA](#3-externe-systeem-kbo-via-magda)
4. [Locatie Synchronisatie Flow](#4-locatie-synchronisatie-flow)
5. [Locatie Retrieval en Matching](#5-locatie-retrieval-en-matching)
6. [Data Transformatie](#6-data-transformatie)
7. [Events en State Management](#7-events-en-state-management)
8. [Main Location Logica](#8-main-location-logica)
9. [Technische Implementatie](#9-technische-implementatie)
10. [Error Handling en Edge Cases](#10-error-handling-en-edge-cases)

---

## 1. Overzicht

Het Organisation Registry systeem synchroniseert locatiegegevens met externe systemen, met name met de **KBO (Kruispuntbank van Ondernemingen)** via het **MAGDA** platform. Het systeem ondersteunt twee soorten locatie bronnen en beheert automatisch de synchronisatie van maatschappelijke zetels van organisaties.

### 1.1 Kernfunctionaliteiten

- ✅ Automatische synchronisatie van locaties vanuit KBO
- ✅ Deduplicatie van locaties op basis van adresgegevens
- ✅ Ondersteuning voor zowel KBO-beheerde als handmatig toegevoegde locaties
- ✅ Automatische bepaling van main location
- ✅ Validiteitsperiodes voor locaties
- ✅ Event sourcing voor volledige audit trail

### 1.2 Betrokken Systemen

| Systeem | Rol | Protocol/Interface |
|---------|-----|-------------------|
| **KBO** | Bron van ondernemingsgegevens | Via MAGDA |
| **MAGDA** | Service bus voor gegevensuitwisseling | SOAP/XML |
| **Organisation Registry** | Ontvangende systeem | Internal domain model |

---

## 2. Locatie Bronnen (Sources)

**Locatie**: `src/OrganisationRegistry/Organisation/LocationSource.cs`

Het systeem kent twee soorten locatie bronnen:

```csharp
public class LocationSource : IEquatable<LocationSource>
{
    public static readonly LocationSource Kbo = new("KBO");
    public static readonly LocationSource Wegwijs = new("WEGWIJS");

    public static readonly LocationSource[] All = { Kbo, Wegwijs };
}
```

### 2.1 KBO Source

- **Beheerder**: KBO (via automatische synchronisatie)
- **Type**: Maatschappelijke zetel (registered office)
- **Synchronisatie**: Automatisch via MAGDA
- **Muteerbaar**: Nee - gebruikers kunnen KBO-locaties niet handmatig aanpassen
- **Lifecycle**: Automatisch toegevoegd/verwijderd bij KBO sync

**Kenmerken**:
- IsKboLocation = true
- Source = "KBO"
- LocationTypeId = KboV2RegisteredOfficeLocationTypeId (uit configuratie)
- ValidFrom/ValidTo: van MAGDA adresgegevens

### 2.2 WEGWIJS Source

- **Beheerder**: Handmatig door gebruikers
- **Type**: Alle locatietypes
- **Synchronisatie**: N.v.t.
- **Muteerbaar**: Ja
- **Lifecycle**: Volledig handmatig beheerd

**Kenmerken**:
- IsKboLocation = false
- Source = "WEGWIJS"
- LocationTypeId: Vrij te kiezen
- ValidFrom/ValidTo: Handmatig in te vullen

---

## 3. Externe Systeem: KBO via MAGDA

### 3.1 MAGDA Interfaces

**Locatie**: `src/OrganisationRegistry/Organisation/IMagdaOrganisationResponse.cs`

#### IMagdaOrganisationResponse

Hoofd-interface voor KBO organisatiegegevens:

```csharp
public interface IMagdaOrganisationResponse
{
    IMagdaName FormalName { get; }
    IMagdaName ShortName { get; }
    DateTime? ValidFrom { get; }
    List<IMagdaBankAccount> BankAccounts { get; }
    IMagdaLegalForm? LegalForm { get; }
    IMagdaAddress? Address { get; }              // ← Maatschappelijke zetel!
    IMagdaTermination? Termination { get; }
    IMagdaLegalEntityType LegalEntityType { get; }
}
```

#### IMagdaAddress

Specifieke interface voor adresgegevens:

```csharp
public interface IMagdaAddress
{
    string Country { get; }       // Landnaam (bijv. "België")
    string City { get; }          // Gemeentenaam (bijv. "Gent")
    string ZipCode { get; }       // Postcode (bijv. "9000")
    string Street { get; }        // Straat + huisnummer + bus (bijv. "Korenmarkt 1 bus A")
    DateTime? ValidFrom { get; }  // Startdatum adres
    DateTime? ValidTo { get; }    // Einddatum adres (null = onbeperkt)
}
```

### 3.2 MAGDA Response Implementatie

**Locatie**: `src/OrganisationRegistry.Api/Infrastructure/Magda/MagdaOrganisationResponse.cs`

#### MagdaAddress Parsing

```csharp
public class MagdaAddress : IMagdaAddress
{
    public const string MaatschappelijkeZetelCode = "001";
    public const string TaalcodeNl = "nl";

    public MagdaAddress(AdresOndernemingType adresOndernemingType)
    {
        // Selecteer Nederlandse beschrijving of eerste beschikbare
        var dutchAddressOrFirst = adresOndernemingType.Descripties
            .FirstOrDefault(IsDutch) ?? adresOndernemingType.Descripties.First();

        // Parse componenten
        Country = dutchAddressOrFirst.Adres?.Land?.Naam?.Trim() ?? string.Empty;
        City = dutchAddressOrFirst.Adres?.Gemeente?.Naam?.Trim() ?? string.Empty;
        ZipCode = adresOndernemingType.Gemeente?.PostCode?.Trim() ?? string.Empty;

        // Bouw straatstring: "Straatnaam 123 bus A"
        var streetName = dutchAddressOrFirst.Adres?.Straat?.Naam?.Trim();
        var houseNumber = adresOndernemingType.Huisnummer?.Trim().TrimStart('0');
        var busNumber = string.IsNullOrEmpty(adresOndernemingType.Busnummer)
            ? ""
            : " bus " + adresOndernemingType.Busnummer;

        Street = $"{streetName} {houseNumber}{busNumber}";

        // Parse validiteit
        ValidFrom = ParseKboDate(adresOndernemingType.DatumBegin);
        ValidTo = ParseKboDate(adresOndernemingType.DatumEinde);
    }

    public static MagdaAddress? FromAddressesOrNull(
        AdresOndernemingType[]? adresOndernemingTypes)
    {
        if (adresOndernemingTypes == null || !adresOndernemingTypes.Any())
            return null;

        // Zoek geldige maatschappelijke zetel (type code "001")
        var validAddress = adresOndernemingTypes?
            .Where(IsValidAddress)
            .FirstOrDefault(IsMaatschappelijkeZetel);

        return validAddress == null
            ? null
            : new MagdaAddress(validAddress);
    }

    private static bool IsMaatschappelijkeZetel(AdresOndernemingType a)
        => a.Type?.Code?.Value == MaatschappelijkeZetelCode;

    private static bool IsValidAddress(AdresOndernemingType a)
        => a.Straat != null 
        && a.Huisnummer != null 
        && a.Gemeente != null 
        && a.Land != null;
}
```

**Belangrijke kenmerken**:
- Alleen **maatschappelijke zetel** (type "001") wordt gesynchroniseerd
- Nederlandse beschrijving heeft voorkeur
- Validatie op aanwezigheid van straat, huisnummer, gemeente, land
- Datum format: "yyyy-MM-dd"

---

## 4. Locatie Synchronisatie Flow

### 4.1 Complete Flow Diagram

```
┌─────────────────────────┐
│  KBO Sync Trigger       │
│  (Manual/Scheduled)     │
└────┬────────────────────┘
     │
     │ 1. Fetch KBO data
     ▼
┌────────────────────────────────┐
│  KboOrganisationRetriever      │
│  → MAGDA GeefOnderneming call  │
└────┬───────────────────────────┘
     │
     │ 2. Return IMagdaOrganisationResponse
     ▼
┌────────────────────────────────────┐
│  KboOrganisationCommandHandlers    │
│  → SyncWithKbo()                   │
└────┬───────────────────────────────┘
     │
     │ 3. Extract Address from response
     ▼
┌──────────────────────────┐
│  GetOrAddLocations()     │
│  → Address available?    │
└────┬─────────────────────┘
     │
     ├─ No → Return null
     │
     ├─ Yes → Continue
     │
     │ 4. Check if location exists
     ▼
┌──────────────────────────────┐
│  KboLocationRetriever        │
│  → RetrieveLocation()        │
│  → Match on:                 │
│     - Country                │
│     - City                   │
│     - ZipCode                │
│     - Street                 │
└────┬─────────────────────────┘
     │
     ├─ Found → Use existing Location
     │
     ├─ Not found → Create new Location
     │               └─ New LocationId (GUID)
     │               └─ Store in LocationList table
     │               └─ Session.Add(location)
     │
     │ 5. Build KboRegisteredOffice object
     ▼
┌──────────────────────────────────┐
│  KboRegisteredOffice             │
│  {                               │
│    Location,                     │
│    ValidFrom,                    │
│    ValidTo                       │
│  }                               │
└────┬─────────────────────────────┘
     │
     │ 6. Commit new locations (if any)
     ▼
┌──────────────────────────┐
│  Session.Commit()        │
└────┬─────────────────────┘
     │
     │ 7. Re-Get Organisation (important!)
     ▼
┌──────────────────────────────────────┐
│  Session.Get<Organisation>()         │
│  (fresh instance with events)        │
└────┬─────────────────────────────────┘
     │
     │ 8. Update registered office locations
     ▼
┌────────────────────────────────────────────┐
│  Organisation.                             │
│    UpdateKboRegisteredOfficeLocations()    │
└────┬───────────────────────────────────────┘
     │
     │ 9. Compare with current KBO location
     ▼
┌──────────────────────────────────┐
│  Same LocationId?                │
└────┬─────────────────────────────┘
     │
     ├─ Yes → No change, skip
     │
     ├─ No → Continue
     │
     │ 10. Remove old KBO location (if exists)
     ▼
┌────────────────────────────────────────────┐
│  ApplyChange(                              │
│    KboRegisteredOfficeOrganisation        │
│    LocationRemoved)                        │
└────┬───────────────────────────────────────┘
     │
     │ 11. Add new KBO location (if provided)
     ▼
┌────────────────────────────────────────────┐
│  ApplyChange(                              │
│    KboRegisteredOfficeOrganisation        │
│    LocationAdded)                          │
│  → OrganisationLocationId: new GUID       │
│  → LocationId: from Location entity       │
│  → IsMainLocation: from previous state    │
│  → LocationTypeId: RegisteredOffice       │
│  → ValidFrom/To: from MAGDA               │
└────┬───────────────────────────────────────┘
     │
     │ 12. Final commit
     ▼
┌──────────────────────────┐
│  Session.Commit()        │
└──────────────────────────┘
```

### 4.2 Code Flow

**Locatie**: `src/OrganisationRegistry/Organisation/KboOrganisationCommandHandlers.cs:75-129`

```csharp
private async Task SyncWithKbo(
    OrganisationId organisationId, 
    IUser user, 
    Guid? kboSyncItemId)
{
    // 1. Get configuration entities
    var registeredOfficeLocationType = Session.Get<LocationType>(
        _organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId);

    // 2. Fetch KBO data via MAGDA
    var organisation = Session.Get<Organisation>(organisationId);
    var kboOrganisationResult = await _kboOrganisationRetriever
        .RetrieveOrganisation(user, organisation.KboState.KboNumber!);

    if (kboOrganisationResult.HasErrors)
        throw new KboOrganisationNotFound(kboOrganisationResult.ErrorMessages);

    var kboOrganisation = kboOrganisationResult.Value;

    // 3. Get or create locations from KBO address
    var location = GetOrAddLocations(kboOrganisation.Address);

    // 4. Commit any new locations
    await Session.Commit(user);

    // 5. IMPORTANT: Re-Get organisation for proper event handling
    organisation = Session.Get<Organisation>(organisationId);

    // 6. Update info from KBO
    organisation.UpdateInfoFromKbo(
        kboOrganisation.FormalName.Value, 
        kboOrganisation.ShortName.Value);

    // 7. Update registered office locations (CORE!)
    organisation.UpdateKboRegisteredOfficeLocations(
        location, 
        registeredOfficeLocationType);

    // 8. Update other KBO data
    organisation.UpdateKboFormalNameLabel(kboOrganisation.FormalName, formalNameLabelType);
    organisation.UpdateKboLegalFormOrganisationClassification(...);
    organisation.UpdateKboBankAccount(kboOrganisation.BankAccounts);
    organisation.AddKboLegalEntityType(kboOrganisation.LegalEntityType);

    // 9. Handle termination
    if (kboOrganisation.Termination != null)
        organisation.MarkTerminationFound(kboOrganisation.Termination);

    organisation.MarkAsSynced(kboSyncItemId);

    // 10. Final commit
    await Session.Commit(user);
}
```

---

## 5. Locatie Retrieval en Matching

### 5.1 IKboLocationRetriever Interface

**Locatie**: `src/OrganisationRegistry/Organisation/IKboLocationRetriever.cs`

```csharp
public interface IKboLocationRetriever
{
    Guid? RetrieveLocation(IMagdaAddress address);
}
```

### 5.2 KboLocationRetriever Implementatie

**Locatie**: `src/OrganisationRegistry.Api/Infrastructure/Magda/KboLocationRetriever.cs`

```csharp
public class KboLocationRetriever : IKboLocationRetriever
{
    private readonly IContextFactory _contextFactory;

    public KboLocationRetriever(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Guid? RetrieveLocation(IMagdaAddress address)
    {
        using var context = _contextFactory.Create();
        
        // Match op ALLE vier adrescomponenten
        return context.LocationList
            .FirstOrDefault(l => 
                l.Country == address.Country &&
                l.City == address.City &&
                l.ZipCode == address.ZipCode &&
                l.Street == address.Street)
            ?.Id;
    }
}
```

**Matching Strategie**:
- ✅ **Exact match** vereist op alle 4 velden
- ✅ Case-sensitive matching
- ✅ Whitespace moet exact overeenkomen
- ❌ Geen fuzzy matching
- ❌ Geen normalisatie van straatnamen

**Consequenties**:
- Kleine verschillen in formatting leiden tot nieuwe locatie
- Bijvoorbeeld: "Korenmarkt 1" ≠ "Korenmarkt  1" (dubbele spatie)
- Dit kan leiden tot duplicate locaties voor hetzelfde fysieke adres

### 5.3 AddOrGetLocation Methode

**Locatie**: `KboOrganisationCommandHandlers.cs:140-163`

```csharp
private Location AddOrGetLocation(IMagdaAddress address)
{
    var existingLocationId = _locationRetriever.RetrieveLocation(address);
    Location location;
    
    if (!existingLocationId.HasValue)
    {
        // Create new location
        location = new Location(
            new LocationId(Guid.NewGuid()),
            null,  // CrabLocationId = null (geen CRAB integratie)
            new Address(
                address.Street,
                address.ZipCode,
                address.City,
                address.Country));

        Session.Add(location);
    }
    else
    {
        // Use existing location
        location = Session.Get<Location>(existingLocationId.Value);
    }

    return location;
}
```

**Belangrijke punten**:
- Nieuwe locaties krijgen een nieuwe GUID
- CrabLocationId is altijd null (geen integratie met CRAB)
- Locaties worden onmiddellijk toegevoegd aan de session
- Eerst commit nodig voordat Organisation update

---

## 6. Data Transformatie

### 6.1 Van MAGDA XML naar Domain Model

```
MAGDA XML (AdresOndernemingType)
  │
  │ Parse door MagdaAddress
  ▼
IMagdaAddress
  - Country: string
  - City: string
  - ZipCode: string
  - Street: string
  - ValidFrom: DateTime?
  - ValidTo: DateTime?
  │
  │ Lookup/Create via KboLocationRetriever
  ▼
Location (Entity)
  - Id: Guid
  - CrabLocationId: int? (null)
  - FormattedAddress: string
  - Street: string
  - ZipCode: string
  - City: string
  - Country: string
  │
  │ Wrap in KboRegisteredOffice
  ▼
KboRegisteredOffice (Value Object)
  - Location: Location
  - ValidFrom: DateTime?
  - ValidTo: DateTime?
  │
  │ Apply to Organisation
  ▼
OrganisationLocation (in Organisation State)
  - OrganisationLocationId: Guid
  - OrganisationId: Guid
  - LocationId: Guid
  - FormattedAddress: string
  - IsMainLocation: bool
  - LocationTypeId: Guid?
  - LocationTypeName: string
  - Validity: Period
  - Source: LocationSource ("KBO")
```

### 6.2 Formatted Address

**Locatie**: `src/OrganisationRegistry/Location/Location.cs`

```csharp
public string FormattedAddress
    => $"{Street}, {ZipCode} {City}, {Country}";
```

**Voorbeelden**:
- Input MAGDA: Street="Korenmarkt 1", ZipCode="9000", City="Gent", Country="België"
- Output: "Korenmarkt 1, 9000 Gent, België"

---

## 7. Events en State Management

### 7.1 KBO Location Events

#### KboRegisteredOfficeOrganisationLocationAdded

**Locatie**: `src/OrganisationRegistry/Organisation/Events/KboRegisteredOfficeOrganisationLocationAdded.cs`

```csharp
public class KboRegisteredOfficeOrganisationLocationAdded : 
    BaseEvent<KboRegisteredOfficeOrganisationLocationAdded>
{
    public Guid OrganisationId => Id;
    public Guid OrganisationLocationId { get; }    // Nieuwe GUID
    public Guid LocationId { get; }                // Van Location entity
    public string LocationFormattedAddress { get; }
    public bool IsMainLocation { get; }            // Van previous state
    public Guid? LocationTypeId { get; }           // RegisteredOffice type
    public string LocationTypeName { get; }
    public DateTime? ValidFrom { get; }            // Van MAGDA
    public DateTime? ValidTo { get; }              // Van MAGDA
}
```

**Wanneer triggered**:
- Bij koppeling van organisatie aan KBO (CoupleOrganisationToKbo)
- Bij creatie van organisatie vanuit KBO (CreateOrganisationFromKbo)
- Bij sync met KBO wanneer adres is gewijzigd (SyncOrganisationWithKbo)

#### KboRegisteredOfficeOrganisationLocationRemoved

**Locatie**: `src/OrganisationRegistry/Organisation/Events/KboRegisteredOfficeOrganisationLocationRemoved.cs`

**Wanneer triggered**:
- Bij sync met KBO wanneer adres is gewijzigd
- Bij ontkoppeling van KBO (CancelCouplingWithKbo)
- Bij sync wanneer KBO organisatie geen adres meer heeft

#### KboRegisteredOfficeLocationIsMainLocationChanged

**Locatie**: `src/OrganisationRegistry/Organisation/Events/KboRegisteredOfficeLocationIsMainLocationChanged.cs`

**Wanneer triggered**:
- Wanneer gebruiker manueel de KBO locatie als main/niet-main markeert
- Via UpdateLocation met source=KBO

### 7.2 State Update Logic

**Locatie**: `Organisation.cs:1386-1421`

```csharp
public void UpdateKboRegisteredOfficeLocations(
    KboRegisteredOffice? maybeNewKboRegisteredOffice,
    LocationType registeredOfficeLocationType)
{
    // Check if location actually changed
    if (maybeNewKboRegisteredOffice?.Location.Id == 
        KboState.KboRegisteredOffice?.LocationId)
        return;  // No change, skip

    // Remember if old location was main
    var isMainLocation = KboState.KboRegisteredOffice?.IsMainLocation ?? false;

    // Remove old KBO location if exists
    if (KboState.KboRegisteredOffice != null)
        ApplyChange(
            new KboRegisteredOfficeOrganisationLocationRemoved(
                Id,
                KboState.KboRegisteredOffice.OrganisationLocationId,
                KboState.KboRegisteredOffice.LocationId,
                KboState.KboRegisteredOffice.FormattedAddress,
                KboState.KboRegisteredOffice.IsMainLocation,
                KboState.KboRegisteredOffice.LocationTypeId,
                KboState.KboRegisteredOffice.LocationTypeName,
                KboState.KboRegisteredOffice.Validity.Start,
                KboState.KboRegisteredOffice.Validity.End,
                KboState.KboRegisteredOffice.Validity.End));

    // Add new KBO location if provided
    if (maybeNewKboRegisteredOffice is { } newKboRegisteredOffice)
        ApplyChange(
            new KboRegisteredOfficeOrganisationLocationAdded(
                Id,
                Guid.NewGuid(),  // New OrganisationLocationId
                newKboRegisteredOffice.Location.Id,
                newKboRegisteredOffice.Location.FormattedAddress,
                isMainLocation,  // Inherit from previous
                registeredOfficeLocationType.Id,
                registeredOfficeLocationType.Name,
                newKboRegisteredOffice.ValidFrom,
                new ValidTo()));  // Open-ended
}
```

**Key Points**:
- Preserve `IsMainLocation` status across location changes
- Always remove old before adding new (even if same location)
- OrganisationLocationId changes even if LocationId stays same
- ValidTo is always null (open-ended) for new KBO locations

---

## 8. Main Location Logica

### 8.1 Main Location Rules

Een organisatie kan **maximaal 1 main location** hebben in een gegeven periode.

**Rules**:
1. KBO locatie **kan** main location zijn (via handmatige actie)
2. KBO locatie wordt **niet automatisch** main location
3. Main location status **blijft behouden** bij adreswijziging KBO locatie
4. Gebruikers kunnen KBO locatie **niet verwijderen** (exception: `LocationIsKboLocation`)
5. Gebruikers kunnen KBO locatie **wel** main/niet-main maken

### 8.2 Main Location Check

**Locatie**: `Organisation.cs:1461`

```csharp
if (organisationLocation.IsMainLocation && 
    State.OrganisationLocations
        .OrganisationAlreadyHasAMainLocationInTheSamePeriod(
            organisationLocation, 
            KboState.KboRegisteredOffice))
    throw new OrganisationAlreadyHasAMainLocationInThisPeriod();
```

**Logic**:
- Check alle OrganisationLocations (inclusief KBO)
- Valideer geen overlap in validiteitsperiodes voor main locations
- Exception als overlap gevonden

### 8.3 Main Location Events

#### MainLocationAssignedToOrganisation

Triggered wanneer:
- Nieuwe location wordt main location
- Bestaande location wordt main location (via update)

#### MainLocationClearedFromOrganisation

Triggered wanneer:
- Main location wordt niet-main
- Main location wordt verwijderd
- Main location validiteit loopt af

---

## 9. Technische Implementatie

### 9.1 Database Schema

#### LocationList Table

```sql
CREATE TABLE [OrganisationRegistry].[LocationList]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [CrabLocationId] INT NULL,
    [FormattedAddress] NVARCHAR(500) NOT NULL,
    [Street] NVARCHAR(200) NULL,
    [ZipCode] NVARCHAR(50) NULL,
    [City] NVARCHAR(200) NULL,
    [Country] NVARCHAR(200) NULL
)
```

**Indexes**:
- Primary key op Id
- Mogelijk index op (Country, City, ZipCode, Street) voor snellere lookups

#### OrganisationLocationListItemView

```sql
CREATE TABLE [OrganisationRegistry].[OrganisationLocationList]
(
    [OrganisationLocationId] UNIQUEIDENTIFIER NOT NULL,
    [OrganisationId] UNIQUEIDENTIFIER NOT NULL,
    [LocationId] UNIQUEIDENTIFIER NOT NULL,
    [LocationFormattedAddress] NVARCHAR(500) NOT NULL,
    [IsMainLocation] BIT NOT NULL,
    [LocationTypeId] UNIQUEIDENTIFIER NULL,
    [LocationTypeName] NVARCHAR(500) NULL,
    [ValidFrom] DATETIME2 NULL,
    [ValidTo] DATETIME2 NULL,
    [Source] NVARCHAR(50) NOT NULL,  -- 'KBO' or 'WEGWIJS'
    [IsKboLocation] AS (CASE WHEN [Source] = 'KBO' THEN 1 ELSE 0 END)
)
```

### 9.2 Key Classes Overzicht

| Class | Locatie | Rol |
|-------|---------|-----|
| `Location` | `src/OrganisationRegistry/Location/Location.cs` | Aggregate root voor locatie entity |
| `LocationSource` | `src/OrganisationRegistry/Organisation/LocationSource.cs` | Value object voor bron (KBO/WEGWIJS) |
| `IKboLocationRetriever` | `src/OrganisationRegistry/Organisation/IKboLocationRetriever.cs` | Interface voor locatie lookup |
| `KboLocationRetriever` | `src/OrganisationRegistry.Api/Infrastructure/Magda/KboLocationRetriever.cs` | Implementatie van locatie lookup |
| `IMagdaAddress` | `src/OrganisationRegistry/Organisation/IMagdaOrganisationResponse.cs` | Interface voor MAGDA adresgegevens |
| `MagdaAddress` | `src/OrganisationRegistry.Api/Infrastructure/Magda/MagdaOrganisationResponse.cs` | Parsing van MAGDA XML naar domain model |
| `KboOrganisationCommandHandlers` | `src/OrganisationRegistry/Organisation/KboOrganisationCommandHandlers.cs` | Command handlers voor KBO sync |
| `Organisation` | `src/OrganisationRegistry/Organisation/Organisation.cs` | Aggregate root met sync logica |

### 9.3 Dependency Injection Setup

**Locatie**: `src/OrganisationRegistry.Api/Infrastructure/ApiModule.cs`

```csharp
builder.RegisterType<KboLocationRetriever>()
    .As<IKboLocationRetriever>()
    .InstancePerLifetimeScope();
```

---

## 10. Error Handling en Edge Cases

### 10.1 Exceptions

#### LocationIsKboLocation

**Locatie**: `src/OrganisationRegistry/Organisation/Exceptions/LocationIsKboLocation.cs`

**Wanneer**: Gebruiker probeert KBO locatie te verwijderen

```csharp
throw new LocationIsKboLocation();
```

#### LocationAlreadyCoupledToInThisPeriod

**Wanneer**: Dezelfde locatie wordt twee keer toegevoegd in overlappende periode

```csharp
throw new LocationAlreadyCoupledToInThisPeriod();
```

#### OrganisationAlreadyHasAMainLocationInThisPeriod

**Wanneer**: Twee main locations in overlappende periode

```csharp
throw new OrganisationAlreadyHasAMainLocationInThisPeriod();
```

#### LocationNotFound

**Wanneer**: Locatie ID niet gevonden in database

```csharp
throw new LocationNotFound();
```

### 10.2 Edge Cases

#### Case 1: KBO Adres Verandert Licht

**Scenario**: 
- MAGDA stuurt "Korenmarkt  1" (dubbele spatie)
- Database heeft "Korenmarkt 1" (enkele spatie)

**Gevolg**:
- Match faalt
- Nieuwe Location entity wordt aangemaakt
- Oude OrganisationLocation wordt verwijderd
- Nieuwe OrganisationLocation wordt toegevoegd
- IsMainLocation status wordt behouden

**Oplossing**:
- Normaliseer whitespace in matching logic (toekomstige verbetering)

#### Case 2: KBO Adres Verdwijnt

**Scenario**:
- Organisatie had maatschappelijke zetel
- MAGDA stuurt geen adres meer (`Address = null`)

**Gevolg**:
- `GetOrAddLocations()` returned `null`
- `UpdateKboRegisteredOfficeLocations(null, ...)` wordt aangeroepen
- `KboRegisteredOfficeOrganisationLocationRemoved` event
- Geen nieuwe locatie toegevoegd

#### Case 3: Meerdere Adressen in KBO

**Scenario**:
- KBO heeft meerdere adressen (vestigingen, bijkantoren)

**Gevolg**:
- Alleen maatschappelijke zetel (type "001") wordt gesynchroniseerd
- Andere adressen worden genegeerd
- Gebruikers kunnen handmatig extra locaties toevoegen (Source=WEGWIJS)

#### Case 4: Locatie Wordt Main Location

**Scenario**:
1. KBO locatie wordt toegevoegd (IsMainLocation=false)
2. Gebruiker maakt het main location
3. KBO sync wijzigt adres

**Gevolg**:
- `UpdateKboRegisteredOfficeLocations()` behoudt IsMainLocation=true
- Nieuwe locatie wordt ook main location
- `KboRegisteredOfficeLocationIsMainLocationChanged` event bij manuele wijziging

#### Case 5: Ontkoppeling van KBO

**Scenario**:
- Organisatie is gekoppeld aan KBO
- Gebruiker ontkoppelt (`CancelCouplingWithKbo`)

**Gevolg**:
- KBO locatie wordt **NIET** automatisch verwijderd
- Source blijft "KBO"
- Gebruiker kan locatie niet verwijderen (exception)
- Workaround: Admin moet manueel via database

### 10.3 Validatie Rules

| Rule | Check Moment | Exception |
|------|--------------|-----------|
| KBO locatie niet verwijderbaar | DeleteOrganisationLocation | `LocationIsKboLocation` |
| Geen duplicate locatie in periode | AddOrganisationLocation, UpdateLocation | `LocationAlreadyCoupledToInThisPeriod` |
| Max 1 main location per periode | AddOrganisationLocation, UpdateLocation | `OrganisationAlreadyHasAMainLocationInThisPeriod` |
| Locatie moet bestaan | UpdateLocation, DeleteLocation | `LocationNotFound` |
| Adres velden verplicht voor MAGDA | MagdaAddress parsing | Return `null` if invalid |

---

## 11. Configuratie

### 11.1 Required Configuration Settings

**Locatie**: `appsettings.json` of environment variables

```json
{
  "Kbo": {
    "KboV2RegisteredOfficeLocationTypeId": "guid-here",
    "KboV2FormalNameLabelTypeId": "guid-here",
    "KboV2LegalFormOrganisationClassificationTypeId": "guid-here"
  }
}
```

**KboV2RegisteredOfficeLocationTypeId**:
- LocationType GUID voor "Maatschappelijke zetel"
- Moet vooraf aangemaakt zijn in LocationTypeList
- Gebruikt voor alle KBO locaties

### 11.2 MAGDA Endpoints

```json
{
  "Magda": {
    "GeefOndernemingEndpoint": "https://magda-endpoint/GeefOnderneming",
    "ClientCertificate": "path-to-cert.pfx",
    "ClientCertificatePassword": "password"
  }
}
```

---

## 12. Testing

### 12.1 Unit Tests

**Locatie**: `test/OrganisationRegistry.UnitTests/Organisation/Kbo/`

Key test scenarios:
- `WhenUpdatingKboRegisteredOfficeLocations.cs`: Test adres wijzigingen
- `CreateOrganisationFromKboTests.cs`: Test initiële koppeling
- `UpdateFromKboTests.cs`: Test sync met wijzigingen
- `WhenRemovingTheKboOrganistionLocation.cs`: Test exception bij verwijderen

### 12.2 Test Stubs

**KboLocationRetrieverStub**: Mock voor location retrieval
```csharp
public class KboLocationRetrieverStub : IKboLocationRetriever
{
    private readonly Guid? _locationId;

    public KboLocationRetrieverStub(Guid? locationId = null)
    {
        _locationId = locationId;
    }

    public Guid? RetrieveLocation(IMagdaAddress address)
        => _locationId;
}
```

---

## 13. Future Improvements

### 13.1 Potentiële Verbeteringen

1. **Fuzzy Address Matching**
   - Normaliseer whitespace
   - Handle afkortingen (str. → straat)
   - Phonetic matching voor straatnamen

2. **CRAB Integratie**
   - Koppel locaties aan CRAB ID's
   - Valideer adressen via CRAB
   - Auto-correct adressen

3. **Batch Sync Optimalisatie**
   - Bulk location creates
   - Batch database queries
   - Parallel processing

4. **Audit Trail**
   - Log alle address changes
   - Track waarom location werd aangemaakt/hergebruikt
   - History van address changes in KBO

5. **Main Location Automation**
   - Configureerbaar: auto-set main location voor KBO
   - Smart detection van meest recente locatie
   - Workflow voor main location goedkeuring

---

## Samenvatting

Het Organisation Registry systeem synchroniseert locaties met KBO via MAGDA:

- ✅ **Automatisch**: Maatschappelijke zetels worden automatisch gesynchroniseerd
- ✅ **Deduplicatie**: Bestaande locaties worden hergebruikt op basis van exact address match
- ✅ **Event Sourced**: Volledige audit trail via events
- ✅ **Protected**: KBO locaties kunnen niet handmatig verwijderd worden
- ✅ **Flexible**: Main location status kan aangepast worden

**Belangrijkste Kenmerken**:
- Alleen type "001" (maatschappelijke zetel) wordt gesynchroniseerd
- Exact matching op Country, City, ZipCode, Street
- IsMainLocation status blijft behouden bij adreswijzigingen
- ValidTo is altijd null (open-ended) voor nieuwe KBO locaties

---

**Document Einde**
