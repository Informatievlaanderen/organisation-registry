# Feature Specification: Permission-Based Authorization

**Feature Branch**: `009-permission-based-authz`
**Created**: 2026-08-27
**Status**: Draft
**Input**: User description: "Transformeer de rol-gebaseerde autorisatie naar een permissie-gebaseerd model. Rollen worden aan de rand van het systeem (edit api, token exchange, bearer) vertaald naar een set van permissies. Controllers checken enkel de algemene permissie (bv. canEditChildren), policies checken enkel nog de restricties (scope). Voorbeeld: `algemeenbeheerder` → `canEditChildren`, `canAddLocations`, ...; `decentraalbeheerder` → `canEditChildren` met scope-restricties."

## Clarifications

### Session 2026-08-27

- Q: Ophaalstrategie voor scope-restricties op resource-niveau (org/orgaan/regelgeving-ids die tot 100 items kunnen bevatten per gebruiker) → A: Just-in-time per policy-evaluatie, niet in het gecachte security-user-object opnemen; request-scoped memoisatie.
- Q: Databron voor JIT-restricties → A: SQL Server-projecties (bestaande read-models), niet ElasticSearch en niet event-replay.
- Q: Enforcement-mechanisme voor permissie-check op controllers → A: Uitbreiden van bestaand `[OrganisationRegistryAuthorize]`-attribuut met `RequiredPermissions`-parameter (declaratief, attribuut-based).
- Q: Permissieset voor Client Credentials scope `dv_organisatieregister_info` → A: Nieuwe expliciete permissie `CanReadInfoEndpoints`, enkel gekoppeld aan de Info-scope.
- Q: Casing-conventie voor permissie-identifiers → A: PascalCase overal (`Permission.CanEditChildren` in C#, "CanEditChildren" in docs/UI); geen camelCase-varianten.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Rollen én scopes vertalen naar permissies aan de systeemrand (Priority: P1)

Een gebruiker (mens of geautomatiseerd proces) authenticeert bij het Organisatieregister via één van de drie ingangen: de edit API (interactieve sessie via ACM/IDM, rol-claims), token exchange (rol-claims), of een bearer token via Client Credentials (scope-claims, gebruikt door geautomatiseerde processen zoals MAGDA, KBO-sync, CJM, Orafin). Op dat moment wordt de rolset of de scope-set van de identiteit vertaald naar een concrete set van permissies (bv. `canEditChildren`, `canAddLocations`, `canAddBodies`, `canManageRegulations`). De rest van het systeem werkt vanaf dat punt uitsluitend met permissies, niet met rollen of scopes.

**Why this priority**: Zonder deze vertaalstap kan geen enkele downstream check (controller of policy) permissie-gebaseerd werken. Dit is de fundamentele MVP-slice die het hele model activeert. Alle bestaande rollen én alle bestaande client-credentials-scopes moeten een equivalente permissieset opleveren zodat er geen gedragswijziging optreedt voor eindgebruikers of geautomatiseerde processen bij release.

**Independent Test**: Kan volledig getest worden door voor elke bestaande rol (AlgemeenBeheerder, VlimpersBeheerder, DecentraalBeheerder, RegelgevingBeheerder, OrgaanBeheerder, Developer, CjmBeheerder, Orafin) én elke bestaande scope (`dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_info`, `dv_organisatieregister_testclient`) na authenticatie de resulterende permissieset op te vragen en te vergelijken met een gedocumenteerde mapping-tabel. Levert waarde op omdat de permissieset zichtbaar en auditeerbaar wordt.

**Acceptance Scenarios**:

1. **Given** een gebruiker met rol `algemeenbeheerder` authenticeert via de edit API, **When** de authenticatie afgerond is, **Then** bevat zijn/haar sessie de permissies `canEditChildren`, `canAddLocations`, `canAddBodies`, en alle andere permissies die aan `algemeenbeheerder` gekoppeld zijn.
2. **Given** een gebruiker met rol `decentraalbeheerder` authenticeert via token exchange, **When** de authenticatie afgerond is, **Then** bevat zijn/haar sessie de permissie `canEditChildren` gecombineerd met scope-restricties (welke organisaties hij/zij mag beheren).
3. **Given** een geautomatiseerd proces authenticeert via een bearer token met scope `dv_organisatieregister_cjmbeheerder` (Client Credentials), **When** de token gevalideerd is, **Then** wordt die scope vertaald naar dezelfde permissieset als de rol `cjmbeheerder` en verkrijgt het proces een `IUser` zonder rollen maar met de correcte permissies.
4. **Given** een gebruiker heeft meerdere rollen tegelijk (of een identiteit meerdere scopes), **When** authenticatie afgerond is, **Then** is de resulterende permissieset de unie van de permissies per rol/scope.
5. **Given** een geautomatiseerd proces authenticeert met een onbekende scope, **When** de token gevalideerd is, **Then** wordt dit als fout gelogd en krijgt het proces een lege permissieset (fail-closed), identiek aan het gedrag voor onbekende rollen.

---

### User Story 2 - Controllers checken enkel algemene permissies (Priority: P2)

Ontwikkelaars die nieuwe of bestaande API-endpoints bouwen, checken op controller-niveau uitsluitend de aanwezigheid van een algemene permissie (bv. "mag deze gebruiker organisatie-kinderen bewerken?"). Er wordt op dit niveau geen rolnaam of scope-string meer gebruikt en geen resource-scope-check gedaan. De controller kent enkel de vraag "is deze actie in principe toegestaan voor deze gebruiker/dit proces?".

**Why this priority**: Dit is de leesbaarste, meest voorkomende autorisatie-check in het systeem. Door hem te vereenvoudigen tot één permissie-check per endpoint wordt de code beter onderhoudbaar en worden nieuwe rollen/scopes mogelijk zonder controllers aan te passen. Volgt logisch op US1: controllers hebben pas permissies om te checken zodra US1 werkt.

**Independent Test**: Kan getest worden door één controller-actie te selecteren, de rol-gebaseerde check te vervangen door een permissie-check, en te verifiëren dat alle huidige rollen én scopes die toegang hadden nog steeds toegang krijgen en alle andere geweigerd worden. Levert waarde op omdat het patroon herbruikbaar wordt.

**Acceptance Scenarios**:

1. **Given** een controller-actie vereist de permissie `canEditChildren`, **When** een gebruiker of proces zonder die permissie de actie aanroept, **Then** wordt de request geweigerd (403) vóór de handler uitgevoerd wordt.
2. **Given** een controller-actie vereist de permissie `canEditChildren`, **When** een gebruiker of proces met die permissie de actie aanroept, **Then** wordt de handler uitgevoerd (scope-checks gebeuren pas verderop in policies).
3. **Given** een nieuwe rol of scope wordt toegevoegd die `canEditChildren` bevat, **When** deze identiteit een bestaande controller-actie aanroept, **Then** krijgt zij toegang zonder dat de controllercode gewijzigd hoeft te worden.

---

### User Story 3 - Policies checken enkel restricties/scope (Priority: P3)

Bij het uitvoeren van een specifieke actie op een specifieke resource (bv. een specifieke organisatie bewerken) checken policies uitsluitend nog de scope-restricties: mag deze gebruiker/dit proces déze specifieke organisatie/orgaan/regelgeving bewerken? Ze gaan er impliciet van uit dat de algemene permissie al gevalideerd is op controller-niveau. Bestaande scope-logica (bv. Vlimpers-restricties, decentrale beheersscope, organisatie-hiërarchie) blijft functioneel identiek.

**Why this priority**: Deze splitsing tussen "algemene permissie" en "scope-restrictie" maakt het autorisatiemodel voorspelbaar en test-baar. Ze komt derde omdat de bestaande policies al gedeeltelijk in deze richting werken en de meerwaarde vooral in leesbaarheid en toekomstige uitbreidbaarheid zit. Bestaande scope-logica moet exact hetzelfde blijven werken.

**Independent Test**: Kan getest worden door voor elke bestaande policy te verifiëren dat: (a) de rol-check verwijderd is, (b) de scope-check identiek functioneert, en (c) een gebruiker met de juiste algemene permissie maar buiten scope geweigerd wordt zoals voorheen. Levert waarde op omdat policies eenvoudiger en gerichter worden.

**Acceptance Scenarios**:

1. **Given** een gebruiker heeft `canEditChildren` op controller-niveau, **When** hij/zij een organisatie buiten zijn/haar decentrale scope probeert te bewerken, **Then** weigert de policy de actie op basis van de scope-restrictie.
2. **Given** een gebruiker heeft `canEditChildren` op controller-niveau, **When** hij/zij een organisatie binnen zijn/haar decentrale scope bewerkt, **Then** laat de policy de actie toe.
3. **Given** een Vlimpers-gerelateerde policy, **When** een gebruiker zonder Vlimpers-scope een Vlimpers-organisatie probeert te bewerken, **Then** weigert de policy op basis van dezelfde restrictieregels als voorheen.

---

### Edge Cases

- Wat gebeurt er wanneer een gebruiker een rol óf een geautomatiseerd proces een scope heeft die (nog) niet gemapt is naar een permissieset? De vertaalstap moet dit expliciet detecteren en de identiteit behandelen als een lege permissieset (weigering), niet stil laten falen. Het onbekende item wordt als fout gelogd via Serilog.
- Hoe wordt omgegaan met een gebruiker die tijdens een lopende sessie een rol verliest? De permissieset wordt bepaald bij authenticatie; wijzigingen tijdens de sessie worden pas actief bij herauthenticatie (identiek aan het huidige gedrag).
- Wat gebeurt er wanneer twee rollen (of rol + scope, of twee scopes) conflicterende scope-restricties leveren (bv. één rol geeft globale scope, één rol geeft beperkte scope)? De ruimste scope wint (unie van rechten).
- Hoe wordt de gecachte security-informatie per gebruiker geïnvalideerd wanneer de rol/scope-naar-permissie-mapping wijzigt? De cache-invalidatie volgt hetzelfde mechanisme als vandaag voor rol-cache.
- Hoe wordt de migratie uitgevoerd? De transformatie gebeurt in één cutover-release: bij deployment verdwijnen alle interne rol-checks en scope-string-checks tegelijk en worden ze vervangen door permissie-checks. Er is geen overgangsperiode met dubbele checks.
- De rol `AutomatedTask` bestaat vandaag als "vangnet" voor geautomatiseerde processen; in het nieuwe model verdwijnt die rol volledig — geautomatiseerde processen komen enkel binnen via Client Credentials scopes en krijgen hun permissies via `ScopePermissionMap`.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Het systeem MOET bij elke succesvolle authenticatie (edit API, token exchange, bearer token) de rolset OF de scope-set van de identiteit vertalen naar een expliciete permissieset.
- **FR-002**: Het systeem MOET een gedocumenteerde, versionbeheerste mapping bijhouden van elke bestaande rol naar de bijhorende permissies (`RolePermissionMap`) én van elke bestaande Client Credentials scope naar de bijhorende permissies (`ScopePermissionMap`).
- **FR-003**: De vertaalstap MOET voor alle drie de authenticatie-ingangen dezelfde mappingtabellen gebruiken, zodat een rol/scope steeds dezelfde permissieset oplevert ongeacht het authenticatiepad.
- **FR-004**: Wanneer een identiteit meerdere rollen of meerdere scopes heeft, MOET de resulterende permissieset de unie zijn van de permissies per rol/scope.
- **FR-005**: Wanneer een identiteit meerdere rollen heeft met scope-restricties, MOET de resulterende scope de ruimste combinatie zijn (bv. een gebruiker met zowel globale als beperkte scope krijgt globale scope).
- **FR-006**: Controllers MOETEN autorisatie uitsluitend uitdrukken als een check op één of meer algemene permissies, zonder verwijzing naar rolnamen of scope-strings. De check MOET declaratief gebeuren via het bestaande `[OrganisationRegistryAuthorize]`-attribuut, uitgebreid met een `RequiredPermissions`-parameter (bv. `[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanEditChildren })]`).
- **FR-007**: Controllers MOETEN geen scope-restricties (op resource-niveau) checken; scope-restricties horen thuis in policies.
- **FR-008**: Policies MOETEN uitsluitend nog scope-restricties evalueren en MOETEN geen rolnaam-checks of scope-string-checks meer bevatten.
- **FR-009**: Het systeem MOET voor elke bestaande rol én elke bestaande Client Credentials scope na de transformatie identieke effectieve toegangsrechten leveren als voor de transformatie (nul functionele regressie voor eindgebruikers en geautomatiseerde processen bij release).
- **FR-010**: Wanneer een rol of scope wordt aangetroffen die niet in de mapping-tabel voorkomt, MOET het systeem dit als fout loggen en de identiteit als "geen permissies" behandelen (fail-closed).
- **FR-011**: De permissienamen MOETEN uitgedrukt worden als expliciete, sprekende identifiers in domeintaal (bv. `canEditChildren`, `canAddLocations`, `canAddBodies`, `canManageRegulations`).
- **FR-012**: Het systeem MOET toelaten om nieuwe permissies toe te voegen aan de mapping-tabellen zonder wijziging aan controllers of policies die deze permissie nog niet gebruiken.
- **FR-013**: Auditlogging van authenticatie/autorisatie valt buiten scope van deze feature; bestaande logging blijft ongewijzigd.
- **FR-014**: De volledige set permissienamen en hun betekenis MOET gedocumenteerd zijn op één centrale plek die later door zowel backend als UI/documentatie geraadpleegd wordt.
- **FR-015**: Rollen én scopes blijven aankomen vanuit ACM/IDM als input van het authenticatiepad, maar worden in de eigen code direct omgezet naar permissies. Na de vertaalstap MOETEN rollen én scope-strings volledig verdwijnen uit het interne security-model: het gecachte security-object bevat uitsluitend permissies, geen rollen, scope-strings, of resource-niveau scope-restricties (org/orgaan/regelgeving-ids). Cache-invalidatie van het permissie-object volgt hetzelfde mechanisme als vandaag voor rol-cache.
- **FR-016**: De rol `AutomatedTask` MOET verdwijnen uit het model. Geautomatiseerde processen krijgen hun permissies uitsluitend via `ScopePermissionMap` op basis van de Client Credentials scope die ACM/IDM meestuurt in het `scope`-claim.
- **FR-017**: Scope-restricties op resource-niveau (welke concrete organisaties/organen/regelgeving een identiteit mag bewerken) MOETEN just-in-time worden opgehaald op het moment dat een policy ze nodig heeft, niet vooraf geladen in het security-user-object. Binnen één HTTP-request MOET de opgehaalde restrictieset gememoiseerd worden (request-scoped) zodat meerdere policy-evaluaties in dezelfde request de data hoogstens één keer laden per soort restrictie.
- **FR-018**: JIT-restricties MOETEN worden opgehaald uit de bestaande SQL Server-projecties (read-models), niet uit ElasticSearch en niet via event-store replay, om consistentie met detail-views te garanderen en indexlatency uit de autorisatiebeslissing te houden.

### Key Entities

- **Rol**: Bestaande domein-identifier voor een groep menselijke gebruikers (bv. `algemeenbeheerder`, `decentraalbeheerder`). Blijft aankomen vanuit ACM/IDM als input van de authenticatie (edit API en token exchange), maar verdwijnt volledig uit het interne security-model nadat de vertaalstap heeft plaatsgevonden.
- **Scope (Client Credentials)**: Bestaande ACM/IDM `scope`-claim-waarde die een geautomatiseerd proces identificeert (bv. `dv_organisatieregister_cjmbeheerder`, `dv_organisatieregister_orafinbeheerder`, `dv_organisatieregister_info`, `dv_organisatieregister_testclient`). Blijft aankomen vanuit ACM/IDM als input van bearer-token-authenticatie, maar verdwijnt volledig uit het interne security-model nadat de vertaalstap heeft plaatsgevonden.
- **Permissie**: Nieuwe, expliciete identifier die aangeeft welke algemene actie een gebruiker of proces mag uitvoeren (bv. `canEditChildren`, `canAddLocations`). Vervangt de rol-check en de scope-string-check in controllers en policies. Bevat geen resource-scope-informatie.
- **Permissieset**: De volledige verzameling permissies die aan één geauthenticeerde identiteit gekoppeld zijn tijdens een sessie. Wordt afgeleid van de rolset via `RolePermissionMap` of van de scope-set via `ScopePermissionMap`, dan samengevoegd via unie.
- **Rol-naar-permissie mapping**: De centrale, gedocumenteerde tabel die per rol vastlegt welke permissies eraan gekoppeld zijn. Enige plek waar rolnamen intern nog voorkomen na de vertaalstap.
- **Scope-naar-permissie mapping**: De centrale, gedocumenteerde tabel die per Client Credentials scope vastlegt welke permissies eraan gekoppeld zijn. Enige plek waar scope-strings intern nog voorkomen na de vertaalstap.
- **Scope-restrictie (resource-niveau)**: De beperking op welke concrete resources (organisaties, organen, regelgeving) een identiteit met een bepaalde permissie mag aanpassen. Blijft functioneel identiek en wordt in policies afgedwongen. Wordt just-in-time opgehaald bij policy-evaluatie (request-scoped gememoiseerd) en NIET in het security-user-object gecached, omdat een gebruiker tientallen tot 100 organisaties in scope kan hebben. Voorbeelden: decentrale beheersscope (organisatiehiërarchie), Vlimpers-scope, orgaan-scope. Niet te verwarren met de Client Credentials `scope`-claim.
- **Policy**: Regel die op resource-niveau bepaalt of een reeds algemeen-gemachtigde identiteit déze specifieke resource mag bewerken. Na de transformatie enkel nog scope-restricties, geen rol-checks of scope-string-checks.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% van de bestaande rollen én Client Credentials scopes heeft na release een equivalente permissieset en behoudt identieke effectieve toegang (nul functionele regressie gemeten via bestaande autorisatie-tests + nieuwe scope-tests).
- **SC-002**: 100% van de controller-endpoints die vandaag een rol-check of scope-string-check doen, doet na release uitsluitend nog een permissie-check (meetbaar via statische code-analyse op verwijzingen naar rol-identifiers en scope-constants in controllerlagen).
- **SC-003**: 100% van de policies bevat na release geen enkele rol-naam of scope-string meer; enkel scope-logica op resource-niveau (meetbaar via code-review checklist per policy).
- **SC-004**: Een nieuwe rol of scope toevoegen (met bestaande permissies) vereist wijzigingen in exact één artefact: de betreffende mapping-tabel (`RolePermissionMap` of `ScopePermissionMap`). Geen wijzigingen aan controllers of policies zijn nodig.
- **SC-005**: Nieuwe ontwikkelaars kunnen aan de hand van de centrale permissie-documentatie binnen 15 minuten bepalen welke permissie een nieuw endpoint moet vereisen (te valideren via onboardings-oefening).
- **SC-006**: Het aantal codepaden waarin een rolnaam of scope-string voorkomt daalt van het huidige niveau (rolcheck + `SecurityService.GetRequiredUser` scope-dispatch + `AutomatedTask`-mapping verspreid over controllers, policies, security-helpers) naar exact twee plaatsen (`RolePermissionMap` + `ScopePermissionMap`).
