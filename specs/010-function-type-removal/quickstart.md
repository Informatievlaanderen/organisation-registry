# Quickstart: Functietype verwijderen verifiëren

## Tests

```bash
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj \
  --filter "FullyQualifiedName~FunctionType|FullyQualifiedName~RemoveBodyMandate|FullyQualifiedName~RemoveOrganisationFunction"
dotnet test test/OrganisationRegistry.Api.IntegrationTests --filter "FullyQualifiedName~FunctionType"
```

De tests moeten aantonen:
- `RemoveFunctionType` levert `FunctionTypeRemoved`; een tweede keer levert geen event; zonder permissie volgt een weigering
- `RemoveBodyMandate` levert `BodyMandateRemoved`; bij een onbekend mandaat volgt geen event; de post blijft bestaan
- `RemoveOrganisationFunction` voor een onbekende functie levert geen exceptie
- de `SyncRemovedItemsService`-gebruiker mag `RemoveOrganisationFunction` en `RemoveBodyMandate` uitvoeren

## Manueel (`SyncRemovedItemsService` aan, delay 10s in development)

1. Maak functietype `X` aan. Koppel het aan een organisatie-functie, aan een organisatie-hoedanigheid met functietype `X`, en via een orgaanpost aan een functietype-mandaat met een persoon aangeduid.
2. `DELETE /v1/functiontypes/{X}` geeft 204.
3. `GET /v1/functiontypes` toont `X` niet. Met `showAll` staat `X` erin met `isRemoved: true`.
4. Wacht één cyclus. De organisatie-functie, de hoedanigheid, het mandaat en de aanduiding zijn weg uit SQL en ES. De orgaanpost bestaat nog.
5. Een volgende cyclus geeft geen fouten in de log.
