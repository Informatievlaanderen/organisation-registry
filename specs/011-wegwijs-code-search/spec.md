# Feature Specification: Zoeken op code (OVO, KBO, Orafin) op de hoofdpagina

**Feature Branch**: `feat/or-3785-wegwijs-code-search`
**Spec Directory**: `specs/011-wegwijs-code-search`
**Created**: 2026-11-10
**Status**: Approved
**Jira**: OR-3785
**Input**: OR-3785 — de zoekfunctie op de hoofdpagina van Wegwijs krijgt naast de huidige velden een veld **Code** dat zoekt op OVO-nummer, KBO-nummer en Orafin-sleutelwaarde. Hiervoor komt er een eigen kopie van de `OrganisationList`-projectie. Aandachtspunt: het Orafin-sleuteltype is configureerbaar en dus niet vast.

## Context

De hoofdpagina van Wegwijs toont het organisatie-overzicht met een zoekfilter. Vandaag bestaat
dat filter uit **Naam**, **OVO-nummer**, **Enkel actieve organisaties**, **Toepassingsgebied**,
**Classificatietype** en **Classificatie**. De lijst wordt bediend door de scherm-API voor
organisaties, die leest uit het read model `OrganisationList`.

Gebruikers hebben in de praktijk meestal één identificerende code in handen — een OVO-nummer,
een KBO-nummer, of de Orafin-code van een entiteit — maar moeten vandaag weten *welk soort*
code ze vasthebben en in welk veld die thuishoort. Enkel OVO-nummer is doorzoekbaar;
KBO-nummer en Orafin-code zijn op de hoofdpagina niet doorzoekbaar.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Zoeken op één code zonder te weten welk type (Priority: P1)

Een gebruiker heeft een code in handen (OVO-nummer, KBO-nummer of Orafin-code) en vult die in
het nieuwe veld **Code** op de hoofdpagina in. Het overzicht toont de organisatie(s) waarvan één
van die drie codes overeenkomt, zonder dat de gebruiker moet weten welk soort code het is.

**Why this priority**: Dit is de kern van het ticket en levert op zichzelf al de volledige
gebruikerswaarde: één invoerveld dat drie codetypes dekt.

**Independent Test**: Vul achtereenvolgens een gekend OVO-nummer, een gekend KBO-nummer en een
gekende Orafin-code in het veld Code in; elke keer verschijnt exact de verwachte organisatie.

**Acceptance Scenarios**:

1. **Given** een organisatie met OVO-nummer `OVO001234`, **When** een gebruiker `OVO001234` in het veld Code invult, **Then** bevat het resultaat die organisatie.
2. **Given** een organisatie met KBO-nummer `0123456789`, **When** een gebruiker dat KBO-nummer in het veld Code invult, **Then** bevat het resultaat die organisatie.
3. **Given** een organisatie met een Orafin-sleutel met waarde `ABC123`, **When** een gebruiker `ABC123` in het veld Code invult, **Then** bevat het resultaat die organisatie.
4. **Given** een code die bij geen enkele organisatie voorkomt, **When** een gebruiker die in het veld Code invult, **Then** is het resultaat leeg en verschijnt geen fout.
5. **Given** een organisatie zonder KBO-nummer en zonder Orafin-sleutel, **When** een gebruiker zoekt op haar OVO-nummer, **Then** wordt die organisatie nog steeds gevonden.

---

### User Story 2 - Code combineren met de bestaande filtervelden (Priority: P2)

Een gebruiker combineert het veld Code met de bestaande filtervelden (naam, enkel actieve
organisaties, toepassingsgebied, classificatietype, classificatie) en krijgt het doorsnede-resultaat.

**Why this priority**: Nodig om het nieuwe veld volwaardig deel te laten zijn van het bestaande
filter, maar de waarde van User Story 1 bestaat ook zonder combinaties.

**Independent Test**: Zoek op een code die bij een inactieve organisatie hoort, één keer met en
één keer zonder "enkel actieve organisaties", en controleer dat de resultaten verschillen zoals
verwacht.

**Acceptance Scenarios**:

1. **Given** een organisatie die matcht op code maar niet op de ingevulde naam, **When** beide velden samen ingevuld zijn, **Then** verschijnt die organisatie niet in het resultaat.
2. **Given** een beëindigde organisatie die matcht op code, **When** "enkel actieve organisaties" aanstaat, **Then** verschijnt die organisatie niet; **When** die filter uitstaat, **Then** verschijnt ze wel.
3. **Given** enkel het veld Code is ingevuld, **When** de gebruiker zoekt, **Then** wordt de zoekopdracht aanvaard (het filter beschouwt Code als een volwaardig zoekcriterium).
4. **Given** het veld Code is leeg, **When** de gebruiker zoekt op de overige velden, **Then** is het gedrag identiek aan vandaag.

---

### User Story 3 - Codes blijven correct na wijzigingen in de bron (Priority: P2)

Wijzigt een code in het register (KBO-koppeling toegevoegd of opgezegd, Orafin-sleutel
toegevoegd, gewijzigd, verwijderd of vervallen, organisatie beëindigd), dan volgt het
zoekgedrag op de hoofdpagina die wijziging.

**Why this priority**: Zonder dit levert het nieuwe veld stille foute resultaten op; het is
echter pas zinvol nadat US1 bestaat.

**Independent Test**: Voeg een Orafin-sleutel toe aan een organisatie, zoek erop (gevonden),
verwijder de sleutel, zoek opnieuw (niet meer gevonden).

**Acceptance Scenarios**:

1. **Given** een organisatie zonder Orafin-sleutel, **When** er een Orafin-sleutel wordt toegevoegd, **Then** is de organisatie kort daarna vindbaar op die waarde.
2. **Given** een organisatie met Orafin-sleutel `A`, **When** de waarde gewijzigd wordt naar `B`, **Then** is ze vindbaar op `B` en niet meer op `A`.
3. **Given** een organisatie met een Orafin-sleutel, **When** die sleutel verwijderd wordt, **Then** is ze niet langer vindbaar op die waarde.
4. **Given** een organisatie die aan KBO gekoppeld wordt, **When** de koppeling tot stand komt, **Then** is ze vindbaar op haar KBO-nummer; **When** de koppeling opgezegd wordt, **Then** niet meer.
5. **Given** een Orafin-sleutel waarvan de geldigheidsperiode verstreken is, **When** een gebruiker op die waarde zoekt, **Then** wordt de organisatie gevonden (de geldigheidsperiode van de sleutel speelt geen rol).
6. **Given** een Orafin-sleutel met een geldigheidsperiode die volledig in de toekomst ligt, **When** een gebruiker op die waarde zoekt, **Then** wordt de organisatie gevonden.
7. **Given** dezelfde gegevens op twee verschillende dagen, **When** een gebruiker op een code zoekt, **Then** is het resultaat identiek (het zoekresultaat is datum-onafhankelijk).

---

### Edge Cases

- **Codetype-verwarring**: eenzelfde ingegeven waarde komt voor als OVO-nummer bij organisatie A en als Orafin-code bij organisatie B → beide organisaties verschijnen in het resultaat (geen prioritering, geen stille onderdrukking).
- **Schrijfwijze van KBO-nummers**: KBO-nummers circuleren als `0123456789`, `0123.456.789` en `BE0123456789`; alle drie leiden tot dezelfde match (FR-004).
- **Deelcodes**: `00` vindt `OVO000001`; `4` vindt elke organisatie met een 4 in een van haar codes. Een korte term geeft dus veel resultaten; dat is gewenst.
- **Hoofdletters**: `ovo001234` versus `OVO001234`; Orafin-codes met letters.
- **Witruimte**: codes die met kopieer-plakken voorafgaande of volgende spaties meekrijgen, worden getrimd.
- **Meerdere Orafin-sleutels**: een organisatie heeft (historisch) meerdere Orafin-sleutels, al dan niet vervallen; elk van die waarden leidt tot een match op die organisatie.
- **Toepassingsgebied-rijen**: het read model bevat per organisatie ook rijen per toepassingsgebied; zoeken op code mag een organisatie niet dubbel laten verschijnen.
- **Autorisatie**: zoeken op code mag geen organisaties of codes zichtbaar maken die de gebruiker vandaag al niet mag zien; de bestaande zichtbaarheidsregels van het overzicht blijven gelden.
- **Herconfiguratie van het Orafin-sleuteltype**: het Orafin-sleuteltype is een gewoon, configureerbaar sleuteltype (instelling `Api:Orafin_KeyTypeId`) en geen vast domeinbegrip. Wordt die instelling gewijzigd, dan moeten de zoekresultaten die wijziging volgen zonder manuele data-ingreep (zie FR-007).
- **Lege of enkel-witruimte code**: wordt behandeld als "niet ingevuld".

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: De scherm-API voor het organisatie-overzicht MOET een bijkomend filterveld `code` aanvaarden, naast de bestaande velden (naam, OVO-nummer, enkel actieve organisaties, toepassingsgebied, classificatietype, classificatie).
- **FR-002**: Bij een ingevuld `code`-filter MOET het systeem organisaties teruggeven waarvan het OVO-nummer, het KBO-nummer of een Orafin-sleutelwaarde overeenkomt met de ingegeven waarde (OR-logica over de drie bronnen).
- **FR-003**: Het `code`-filter MOET combineerbaar zijn met elk bestaand filterveld; de resultaten zijn de doorsnede (AND) van alle ingevulde criteria.
- **FR-004**: Het `code`-filter MOET een **deelmatch (substring) op genormaliseerde waarde** zijn: een organisatie matcht wanneer de genormaliseerde zoekterm *ergens* in een van haar genormaliseerde codes voorkomt (`4` vindt elke organisatie met een 4 in haar OVO-nummer, KBO-nummer of Orafin-code). Er is **geen fuzzy match** (geen typo-tolerantie). Normalisatie betekent: witruimte vooraan en achteraan weg, hoofdletteronafhankelijk, en scheidingstekens genegeerd (`0123.456.789` en `0123456789` zijn dezelfde code). Een zoekterm met Belgische landcode (`BE0123456789`) vindt daarnaast ook het KBO-nummer zonder landcode. Dezelfde normalisatie geldt voor de opgeslagen code én voor de ingegeven zoekterm.
- **FR-005**: Een organisatie MOET maximaal één keer in het resultaat verschijnen, ook wanneer meerdere van haar codes op de zoekterm matchen of wanneer ze meerdere toepassingsgebied-rijen heeft.
- **FR-005b**: **Alle** Orafin-sleutels van een organisatie MOETEN tot een match kunnen leiden, ongeacht hun geldigheidsperiode: ook vervallen en nog niet aangevangen sleutels tellen mee. Het `code`-filter toetst de geldigheidsperiode van een sleutel dus niet.
- **FR-005c**: Het zoekresultaat van het `code`-filter MOET datum-onafhankelijk zijn: dezelfde gegevens geven op elke dag hetzelfde resultaat. De bestaande filter "enkel actieve organisaties" blijft het enige tijdsgebonden criterium, en werkt zoals vandaag op de geldigheid van de **organisatie**, niet op die van een sleutel.
- **FR-006**: Het resultaat van het overzicht MOET dezelfde kolommen, velden en sortering als vandaag behouden; het `code`-filter voegt enkel een selectiecriterium toe. KBO-nummer en Orafin-code worden NIET aan het resultaat toegevoegd.
- **FR-007**: Wijzigt de instelling die bepaalt welk sleuteltype "Orafin" is (`Api:Orafin_KeyTypeId`), dan MOETEN de zoekresultaten die nieuwe instelling volgen zonder dat een projectie herbouwd moet worden.
- **FR-008**: Het systeem MOET het zoekgedrag laten volgen op wijzigingen in de onderliggende gegevens: toegekende en opgezegde KBO-koppelingen, toegevoegde, gewijzigde en verwijderde organisatiesleutels, hernoemde of verwijderde sleuteltypes, en beëindigde organisaties.
- **FR-009**: Het bestaande gedrag van het organisatie-overzicht (zonder `code`-filter) MOET onveranderd blijven: zelfde resultaten, zelfde paginering, zelfde sortering, zelfde autorisatieregels.
- **FR-010**: Codes die een gebruiker vandaag niet mag zien, MOGEN via het `code`-filter geen organisaties blootgeven die buiten zijn zichtbaarheid vallen.
- **FR-011**: Het nieuwe read model MOET volledig herbouwbaar zijn uit de eventstroom, met de bestaande herbouw-mogelijkheid voor projecties.
- **FR-012**: De zoekfilter-UI op de hoofdpagina MOET een invoerveld **Code** aanbieden en dat veld als geldig zoekcriterium beschouwen (zoeken met enkel Code ingevuld is toegelaten).

### Constraints *(vastgelegd in OR-3785)*

- **C-001**: Deze functionaliteit krijgt een **eigen read model**: een kopie van de huidige
  `OrganisationList`-projectie, specifiek voor het zoeken op de hoofdpagina. De bestaande
  `OrganisationList`-projectie en haar bestaande consumenten blijven ongewijzigd.
- **C-002**: De Orafin-sleutel wordt bepaald via het **geconfigureerde sleuteltype-id**
  (`Api:Orafin_KeyTypeId`), conform het bestaande patroon in de rapportering, en niet via de
  naam van het sleuteltype of via "alle sleuteltypes".
- **C-003**: Het zoeken op de hoofdpagina blijft lopen over het SQL-read model; deze feature
  voegt géén ElasticSearch-index toe.

### Key Entities *(include if feature involves data)*

- **Zoek-read model voor organisaties**: per organisatie (en per toepassingsgebied-variant, zoals
  vandaag) de gegevens die de hoofdpagina nodig heeft om te filteren en te tonen: identificatie,
  OVO-nummer, naam, korte naam, moederentiteit, geldigheidsperiode, toepassingsgebied,
  classificatie-geldigheden — aangevuld met de codes waarop gezocht kan worden.
- **Organisatiecode**: een doorzoekbare code die bij een organisatie hoort, met haar herkomst
  (OVO-nummer, KBO-nummer, of een sleutelwaarde met bijhorend sleuteltype) en haar
  geldigheidsperiode waar die bestaat.
- **Orafin-sleuteltype-instelling**: de configuratie die aanduidt welk sleuteltype als "Orafin"
  geldt; dynamisch en wijzigbaar buiten een release om.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Voor elk van de drie codetypes (OVO-nummer, KBO-nummer, Orafin-code) vindt een gebruiker de juiste organisatie met één zoekopdracht in één veld, zonder voorkennis van het codetype.
- **SC-002**: 100% van de organisaties met een KBO-nummer is vindbaar op dat KBO-nummer, en 100% van de organisaties met een in aanmerking komende Orafin-sleutel is vindbaar op die waarde (meetbaar door steekproef na herbouw van het read model).
- **SC-003**: Een wijziging aan een KBO-koppeling of een Orafin-sleutel is binnen de gebruikelijke projectie-achterstand (zelfde orde als de bestaande overzichtslijst) weerspiegeld in de zoekresultaten.
- **SC-004**: Een wijziging van de instelling `Api:Orafin_KeyTypeId` is zichtbaar in de zoekresultaten na een herstart van de API (die de configuratie bij het opstarten inleest), zonder herbouw van een projectie en zonder release.
- **SC-005**: Zoekopdrachten op de hoofdpagina zijn met het nieuwe veld niet merkbaar langzamer dan vandaag (geen regressie in de responstijd van het overzicht bij vergelijkbare filters).
- **SC-006**: De bestaande zoek- en overzichtsfunctionaliteit vertoont geen regressie: alle bestaande geautomatiseerde tests voor het organisatie-overzicht blijven slagen.

## Resolved Clarifications

1. **Match-gedrag van het `code`-veld** (FR-004): deelmatch (substring) op genormaliseerde waarde,
   zoals gevraagd in OR-3785 ("als ik 4 ingeef, krijg ik alle organisaties met een 4 in de OVO-code,
   het KBO-nummer of het Orafin-nummer"). Geen fuzzy match. *(oorspronkelijk beslist als exacte match
   op 2026-11-10; herzien 2026-11-26 na vaststelling dat dit het ticket niet dekte)*
2. **Vervallen sleutelwaarden** (FR-005b, FR-005c): alle Orafin-sleutels tellen mee, ook
   vervallen en toekomstige; de geldigheidsperiode van een sleutel wordt niet getoetst, en het
   zoekresultaat is datum-onafhankelijk. Het Orafin-sleuteltype zelf blijft bepaald door de
   instelling `Api:Orafin_KeyTypeId` (C-002). *(beslist 2026-11-10, herzien 2026-11-10)*
3. **Resultaatkolommen** (FR-006): de kolommen van het overzicht blijven ongewijzigd; KBO-nummer
   en Orafin-code komen niet in het resultaat. *(beslist 2026-11-10)*

## Open Clarifications

Geen.
