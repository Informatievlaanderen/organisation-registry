# Data Model: Zoeken op code (OVO, KBO, Orafin)

**Feature**: `specs/011-wegwijs-code-search` | **Plan**: [plan.md](./plan.md) | **Jira**: OR-3785

Alle tabellen in schema `WellknownSchemas.BackofficeSchema`, zoals het bestaande read model.

## 1. `OrganisationSearchList` — kopie van `OrganisationList`

Entiteit `OrganisationSearchListItem`, kolom-voor-kolom gelijk aan `OrganisationListItem`
(`src/OrganisationRegistry.SqlServer/Organisation/OrganisationListItemView.cs:19`):

| Kolom | Type | Opmerking |
|---|---|---|
| `Id` | int identity | PK, niet-geclusterd |
| `OrganisationId` | uniqueidentifier | verplicht |
| `OvoNumber` | nvarchar(10) | verplicht |
| `Name` | nvarchar(500) | verplicht, geclusterde index |
| `ShortName` | nvarchar | |
| `ParentOrganisation` | nvarchar(500) | |
| `ParentOrganisationId` | uniqueidentifier | |
| `ParentOrganisationOvoNumber` | nvarchar | |
| `ParentOrganisationsRelationshipId` | uniqueidentifier | |
| `FormalFrameworkId` | uniqueidentifier | nullable; unieke index `(OrganisationId, FormalFrameworkId)` |
| `ValidFrom` / `ValidTo` | datetime | geldigheid organisatie |

Kindtabellen, identiek aan het origineel (enkel andere tabelnamen):

- `OrganisationSearchFormalFrameworkValidity` ← `OrganisationFormalFrameworkValidity`
- `OrganisationSearchClassificationValidity` ← `OrganisationClassificationValidity`

Beide met cascade delete vanuit de parent, en dezelfde indexen op `ValidFrom` / `ValidTo` /
`OrganisationClassificationTypeId`.

> Geen `Code`-kolom op deze tabel: codes horen bij de organisatie, niet bij de
> organisatie-per-toepassingsgebied-rij (zie §2 en D-001 in het plan).

## 2. `OrganisationSearchCodeList` — doorzoekbare codes

Entiteit `OrganisationSearchCodeListItem`. Één rij per doorzoekbare code per organisatie.

| Kolom | Type | Verplicht | Opmerking |
|---|---|---|---|
| `Id` | int identity | ✔ | PK, niet-geclusterd |
| `OrganisationId` | uniqueidentifier | ✔ | geen FK-navigatie naar §1 (daar is `OrganisationId` niet uniek); de query joint via subquery |
| `Source` | nvarchar(10) | ✔ | `Ovo` \| `Kbo` \| `Key` |
| `KeyTypeId` | uniqueidentifier | ✖ | gevuld voor `Source = Key`, null voor `Ovo` en `Kbo` |
| `OrganisationKeyId` | uniqueidentifier | ✖ | identificeert de sleutelrij bij wijzigen/verwijderen |
| `Value` | nvarchar(500) | ✔ | waarde zoals opgeslagen (voor debugging/traceerbaarheid) |
| `NormalizedValue` | nvarchar(500) | ✔ | zoekwaarde, zie §4 |
| `ValidFrom` | datetime | ✖ | **informatief**, niet gebruikt in het filter; null voor OVO en KBO |
| `ValidTo` | datetime | ✖ | **informatief**, niet gebruikt in het filter; null voor OVO en KBO |

**Indexen**

- `(NormalizedValue, KeyTypeId)` — bedient het code-filter
- `(OrganisationId)` — bedient herprojectie en opruiming
- `(OrganisationKeyId)` — bedient wijzigen/verwijderen van een sleutel

**Invarianten**

- Per organisatie maximaal één `Ovo`-rij en maximaal één `Kbo`-rij.
- Een `Key`-rij bestaat exact zolang de bijhorende organisatiesleutel bestaat — ongeacht of die
  sleutel vandaag geldig is (FR-005b).
- `NormalizedValue` is altijd afgeleid van `Value` volgens §4 (nooit manueel gezet).
- Geen enkele kolom bevat een tijdsgebonden beoordeling; het zoekresultaat is datum-onafhankelijk
  (FR-005c).

## 3. Event → mutatie

### `Ovo`-rij

| Event | Mutatie |
|---|---|
| `OrganisationCreated`, `OrganisationCreatedFromKbo` | `Ovo`-rij aanmaken met het OVO-nummer, geldigheid null |

Het OVO-nummer wijzigt niet na creatie; er is dus geen update-pad.

### `Kbo`-rij

Patroon zoals `OrganisationDetailItemView.cs:195`:

| Event | Mutatie |
|---|---|
| `OrganisationCreatedFromKbo` | `Kbo`-rij aanmaken |
| `OrganisationCoupledWithKbo` | `Kbo`-rij aanmaken of bijwerken |
| `OrganisationCouplingWithKboCancelled` | `Kbo`-rij verwijderen |
| `OrganisationTerminationSyncedWithKbo` | `Kbo`-rij verwijderen (zoals `OrganisationDetailItemView` het nummer op null zet) |
| `OrganisationTerminated`, `OrganisationTerminatedV2` | `Kbo`-rij verwijderen wanneer `ForcedKboTermination` waar is (zelfde voorwaarde als `OrganisationDetailItemView.cs:480`) |

### `Key`-rijen

Patroon zoals `OrganisationKeyListItemView.cs`:

| Event | Mutatie |
|---|---|
| `OrganisationKeyAdded` | rij aanmaken met `KeyTypeId`, `OrganisationKeyId`, waarde en (informatief) `ValidFrom`/`ValidTo` uit het event |
| `OrganisationKeyUpdated` | rij bijwerken (waarde, genormaliseerde waarde, geldigheid, sleuteltype) |
| `OrganisationKeyRemoved` | rij verwijderen |
| `KeyTypeRemoved` | rijen van dat sleuteltype verwijderen |
| `OrganisationTerminatedV2` | `ValidTo` van de betrokken sleutels bijwerken volgens `FieldsToTerminateV2.Keys` — informatief; de rij blijft doorzoekbaar (de filter "enkel actieve organisaties" werkt op de organisatie, niet op de sleutel) |

`KeyTypeUpdated` (hernoemen) heeft **geen** effect: de naam van het sleuteltype wordt in dit read
model niet bewaard, enkel het id. Dat is bewust — matchen gebeurt op `KeyTypeId` (C-002).

### Overige kolommen van §1

Identiek aan `OrganisationListItemView`: dezelfde events, dezelfde mutaties. Zie de
eventlijst in `OrganisationListItemView.cs:155`.

## 4. Normalisatieregels

```text
Normalize(s):
  1. trim
  2. hoofdletters (invariant culture)
  3. verwijder alle tekens die geen letter of cijfer zijn
```

Voorbeelden: `" 0123.456.789 "` → `0123456789`; `ovo001234` → `OVO001234`;
`ABC-123` → `ABC123`.

**Zoekkandidaten** uit een ingegeven term `t`, gededupliceerd:

```text
Candidates(t):
  c0 = Normalize(t)
  c1 = c0 zonder "BE"-prefix        enkel als c0 matcht op ^BE\d{9,10}$
  c2 = "0" + c0                     enkel als c0 matcht op ^\d{9}$
```

De query vergelijkt met `candidates.Contains(NormalizedValue)` → `IN (…)`, zodat de index
op `NormalizedValue` bruikbaar blijft (geen functie op de kolom).

## 5. Queryvorm van het code-filter

```csharp
if (filter.Code is { } code && code.IsNotEmptyOrWhiteSpace())
{
    var candidates = OrganisationCodeNormalizer.Candidates(code);
    var orafinKeyTypeId = _configuration.OrafinKeyTypeId;

    var matching = _context.OrganisationSearchCodeList
        .Where(c => candidates.Contains(c.NormalizedValue))
        .Where(c => c.KeyTypeId == null || c.KeyTypeId == orafinKeyTypeId)
        .Select(c => c.OrganisationId);

    organisations = organisations.Where(x => matching.Contains(x.OrganisationId));
}
```

- `KeyTypeId == null` dekt OVO en KBO; `== orafinKeyTypeId` dekt enkel het geconfigureerde
  Orafin-sleuteltype (FR-002, C-002, FR-007).
- **Geen geldigheidstest**: alle Orafin-sleutels tellen mee, ook vervallen (FR-005b, D-002).
  Daardoor bevat de query geen datumvergelijking en is het resultaat datum-onafhankelijk (FR-005c).
- Omdat er op `OrganisationId` vergeleken wordt en het basisfilter al één rij per organisatie
  oplevert (`FormalFrameworkId == null`, of één specifiek toepassingsgebied), blijft het
  resultaat vrij van duplicaten (FR-005).
