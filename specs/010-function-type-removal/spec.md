# Feature Specification: Functietype verwijderen (Deleted markeren)

**Feature Branch**: `feat/or-3412-function-type-removal`
**Created**: 2026-10-20
**Status**: Draft
**Jira**: OR-3412
**Input**: User description: "Als algemeen beheerder wil ik functie type kunnen aanduiden als Deleted waardoor deze niet meer in de keuzelijsten verschijnen bij toevoegen van een functie. Gelijkaardig aan huidige opzet voor parameter Hoedanigheden. Acceptatiecriteria: voor functie type wordt opgeslagen of deze deleted is en kan deze vanuit de FE gedelete worden; bij functie toevoegen worden enkel niet-deleted functietypes teruggestuurd; achterliggend loopt een procedure om de organisatie-functie koppeling effectief te verwijderen en ook alle andere linken (Hoedanigheden, Mandaten). Bijkomend: niet enkel koppeling met Organisatie maar ook andere linken met functies verwijderen."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Functietype als verwijderd markeren (Priority: P1)

Een algemeen beheerder opent de parameterlijst "Functietypes" en verwijdert een functietype dat niet meer gebruikt mag worden. Het functietype blijft historisch bewaard maar is gemarkeerd als verwijderd.

**Why this priority**: Kern van het ticket; zonder deze markering is geen van de andere stories mogelijk.

**Independent Test**: Verwijder een functietype via de parameterbeheer-UI en controleer dat het als verwijderd geregistreerd staat en een tweede verwijdering geen effect heeft.

**Acceptance Scenarios**:

1. **Given** een actief functietype, **When** een algemeen beheerder het verwijdert, **Then** wordt het functietype als verwijderd geregistreerd.
2. **Given** een reeds verwijderd functietype, **When** een beheerder het opnieuw verwijdert, **Then** verandert er niets en treedt geen fout op.
3. **Given** een gebruiker zonder rol algemeen beheerder, **When** die een functietype probeert te verwijderen, **Then** wordt de actie geweigerd.

---

### User Story 2 - Verwijderde functietypes niet aanbieden bij keuzelijsten (Priority: P1)

Bij het toevoegen of wijzigen van een functie (en andere schermen waar een functietype gekozen wordt) krijgt de gebruiker enkel niet-verwijderde functietypes te zien.

**Why this priority**: Dit is het zichtbare doel voor eindgebruikers: een beperkte, actuele keuzelijst.

**Independent Test**: Verwijder een functietype en controleer dat de keuzelijst bij "functie toevoegen" het niet meer bevat, terwijl de parameterbeheerlijst het (gemarkeerd) wel nog kan tonen.

**Acceptance Scenarios**:

1. **Given** een verwijderd functietype, **When** de keuzelijst functietypes wordt opgevraagd bij het toevoegen of wijzigen van een organisatie-functie of bij het koppelen van een functietype aan een orgaan-mandaat, **Then** bevat die het verwijderde functietype niet.
2. **Given** de parameterbeheerlijst, **When** de beheerder expliciet verwijderde items wil zien, **Then** kan dat (conform hoedanigheden).

---

### User Story 3 - Bestaande koppelingen opruimen (Priority: P2)

Na het verwijderen van een functietype ruimt een achterliggende procedure alle koppelingen met dat functietype op: organisatie-functies, organisatie-hoedanigheden die naar het functietype verwijzen, en orgaan-mandaten van dat functietype.

**Why this priority**: Vereist voor dataconsistentie, maar mag asynchroon na de markering gebeuren.

**Independent Test**: Maak een functietype met koppelingen naar een organisatie-functie, een organisatie-hoedanigheid en een orgaan-mandaat; verwijder het functietype; laat de procedure lopen; controleer dat alle koppelingen verwijderd zijn en dat dit traceerbaar is in de historiek.

**Acceptance Scenarios**:

1. **Given** organisatie-functies met het verwijderde functietype, **When** de opruimprocedure loopt, **Then** worden die organisatie-functies verwijderd.
2. **Given** hoedanigheden (organisatie-hoedanigheden) die naar het verwijderde functietype verwijzen, **When** de procedure loopt, **Then** worden die koppelingen verwijderd (de volledige organisatie-hoedanigheid wordt verwijderd).
3. **Given** orgaan-mandaten toegewezen aan het verwijderde functietype (inclusief personen aangeduid op die mandaten), **When** de procedure loopt, **Then** worden die mandaten verwijderd; de orgaanpost zelf blijft bestaan.
4. **Given** koppelingen waarvan de geldigheid al verlopen is, **When** de procedure loopt, **Then** worden ook die verwijderd.
5. **Given** de procedure is gelopen, **When** ze opnieuw loopt, **Then** worden geen extra wijzigingen gedaan (idempotent).

---

### Edge Cases

- Koppelingen die al in het verleden afgelopen zijn (niet meer geldig) worden ook verwijderd; geldigheid speelt geen rol.
- Orgaanposten worden niet verwijderd: een post is niet aan een functietype gekoppeld, enkel het mandaat op de post.
- Koppelingen op organisaties die onder KBO-beheer of onder een beperkte (decentrale) bevoegdheid vallen: de procedure draait als systeemgebruiker en moet die toch kunnen verwijderen.
- Een koppeling die na de verwijdering toch nog met het verwijderde functietype wordt gemaakt (bv. rechtstreeks via de API) wordt niet opgeruimd; dat is gelijk aan het gedrag bij hoedanigheden.
- Een verwijderd functietype wordt niet terug actief gemaakt (geen "herstel" in scope).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Het systeem MOET voor een functietype registreren dat het verwijderd is, als nieuw historisch feit (bestaande historiek blijft ongewijzigd).
- **FR-002**: Enkel een algemeen beheerder MAG een functietype verwijderen.
- **FR-003**: Het verwijderen van een reeds verwijderd functietype MOET zonder effect en zonder fout gebeuren.
- **FR-004**: De parameterlijst functietypes MOET standaard enkel niet-verwijderde functietypes teruggeven, met een optie om verwijderde mee op te vragen — gelijk aan het gedrag voor hoedanigheden.
- **FR-005**: Het beheerscherm (FE) voor functietypes MOET een verwijderactie aanbieden die de verwijdering opslaat.
- **FR-006**: Net als bij hoedanigheden wordt een verwijderd functietype enkel uit de keuzelijst gehaald; het systeem weigert koppelingen met een verwijderd functietype niet domeinmatig.
- **FR-007**: Na verwijdering MOETEN alle bestaande koppelingen met het functietype gemarkeerd worden voor verwijdering, en MOET een achterliggende procedure ze effectief verwijderen — conform de bestaande opruimprocedure voor hoedanigheden.
- **FR-008**: De opruimprocedure MOET organisatie-functies, volledige organisatie-hoedanigheden met verwijzing naar het functietype en orgaan-mandaten van het functietype verwijderen — ongeacht geldigheidsperiode (ook historische koppelingen).
- **FR-009**: Elke verwijdering door de procedure MOET traceerbaar zijn in de historiek met de procedure als uitvoerder.
- **FR-010**: Zoek- en detailoverzichten MOETEN na de opruiming de verwijderde koppelingen niet meer tonen.

### Key Entities

- **Functietype**: parameter met naam; krijgt status verwijderd.
- **Organisatie-functie**: koppeling persoon–organisatie met een functietype en geldigheid.
- **Organisatie-hoedanigheid**: koppeling persoon–organisatie met een hoedanigheid; kan optioneel naar een functietype verwijzen.
- **Orgaan-mandaat**: mandaat op een orgaanpost dat aan een functietype (van een organisatie) kan toegewezen zijn; personen kunnen op zo'n mandaat aangeduid worden.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Een algemeen beheerder kan een functietype verwijderen in één actie vanuit het beheerscherm.
- **SC-002**: 100% van de keuzelijsten voor het toevoegen van een functie bevat geen verwijderde functietypes, onmiddellijk na verwijdering.
- **SC-003**: Na één run van de opruimprocedure bestaan er 0 koppelingen meer (binnen de afgesproken scope) met een verwijderd functietype.
- **SC-004**: Een tweede run van de procedure levert 0 bijkomende wijzigingen op.

## Clarifications

### Session 2026-10-20

- Q: Hoedanigheden die naar de functie verwijzen? → A: Hele organisatie-hoedanigheid verwijderen.
- Q: Historische koppelingen? → A: Alles verwijderen, ook historische.
- Q: Orgaanposten met dit functietype? → A: (gecorrigeerd) Een post heeft geen functietype; enkel orgaan-mandaten van dat functietype worden verwijderd, de post blijft.
- Q: Nieuwe koppelingen met een verwijderd functietype weigeren? → A: Nee, hetzelfde als hoedanigheden: enkel de keuzelijst filtert.

## Assumptions

- "Algemeen beheerder" is de bestaande rol die vandaag hoedanigheden mag verwijderen.
- De opruimprocedure hergebruikt het bestaande periodieke opruimmechanisme voor gemarkeerde koppelingen (zoals bij hoedanigheden en sleutels).
- Herstellen van een verwijderd functietype valt buiten scope.
- Bestaande verwijzingen in historiek (events) blijven bewaard; enkel de huidige toestand wordt opgeschoond.
