# Locaties en Externe Bronnen

**Doelgroep**: Product Owner, Business Analisten, Functioneel Beheer
**Laatste update**: 2025-02-25

## Overzicht

Locaties in het organisatieregister komen uit twee bronnen:

1. **KBO** - Automatische synchronisatie van maatschappelijke zetels
2. **Grar/Geo API** - Handmatig hulpmiddel voor adresinvoer

## KBO (Kruispuntbank van Ondernemingen)

**Werking:**
- Bij koppeling aan KBO wordt maatschappelijke zetel automatisch overgenomen via MAGDA
- Synchronisatie houdt adressen up-to-date
- Alleen type "001" (maatschappelijke zetel) wordt gesynchroniseerd

**Eigenschappen:**
- ✅ Automatisch aangemaakt en bijgewerkt
- ✅ Altijd correct en actueel
- ❌ Niet bewerkbaar door gebruikers
- ❌ Kan niet verwijderd worden

**Bron label**: `KBO`

## Grar/Geo API

**Werking:**
- Autocomplete functie bij handmatige aanmaak locaties
- Gebruiker typt adres → krijgt suggesties → velden worden automatisch ingevuld
- Optioneel: gebruiker kan ook volledig handmatig invoeren

**Eigenschappen:**
- ✅ Alleen invoerhulpmiddel (geen synchronisatie)
- ✅ Optioneel - volledig handmatig kan ook
- ✅ Bewerkbaar door gebruikers
- ⚠️ Alleen Vlaanderen
- ℹ️ Grar ID wordt opgeslagen voor referentie

**Bron label**: `WEGWIJS` (met optionele Grar ID)

## Belangrijkste Verschillen

| | KBO | Grar |
|---|---|---|
| **Type** | Automatische synchronisatie | Handmatig hulpmiddel |
| **Doel** | Maatschappelijke zetels | Alle locaties (extra kantoren, etc.) |
| **Update** | Automatisch nachtelijk | Geen - statisch |
| **Bewerkbaar** | Nee | Ja |
| **Bereik** | België + buitenland | Alleen Vlaanderen |
| **Verplicht** | Bij KBO-koppeling | Optioneel |

## Hoe werkt KBO-synchronisatie?

### Locatie bestaat nog niet
Bij KBO-koppeling zoekt het systeem eerst of het adres al bestaat (exacte match op straat, postcode, gemeente, land):

**Niet gevonden:**
1. Nieuwe Location wordt automatisch aangemaakt
2. Adresgegevens komen van KBO/MAGDA
3. Geen Grar-link (GrarLocationId = null)
4. Locatie wordt gekoppeld aan organisatie met type "Registered office"

**Wel gevonden:**
1. Bestaande Location wordt hergebruikt
2. Nieuwe organisatie gebruikt zelfde Location
3. Voorkomt duplicaten

### Locatie bestaat al en wijzigt
Bij adreswijziging in KBO:
1. Oude koppeling krijgt einddatum
2. Systeem zoekt/maakt nieuwe locatie
3. Nieuwe koppeling wordt aangemaakt
4. Hoofdlocatie-status blijft behouden

## Gebruikersscenario's

**Organisatie met KBO-nummer aanmaken**
→ Maatschappelijke zetel komt automatisch (KBO-bron)

**Extra locatie toevoegen aan organisatie**
→ Beheerder maakt handmatig locatie aan, optioneel met Grar-hulp (WEGWIJS-bron)

**Adres wijzigt in KBO**
→ Automatische update in organisatieregister binnen 24u

**Beheerder wijzigt Grar-gelinkte locatie**
→ Grar-link wordt verbroken (blijft handmatig beheerd)

## Business Rules

### Deduplicatie
- Eén adres = één locatie in het systeem
- Bij exacte match (straat, postcode, gemeente, land) wordt bestaande locatie hergebruikt
- Voorkomt duplicaten bij KBO-sync

### KBO-locaties Beschermd
- Kunnen niet verwijderd of bewerkt worden door gebruikers
- Alleen KBO-synchronisatie mag deze wijzigen
- Voorkomt inconsistentie met officiële bedrijfsgegevens

### Hoofdlocatie bij KBO-update
- Bij adreswijziging in KBO blijft hoofdlocatie-status behouden
- Oude locatie krijgt einddatum, nieuwe wordt hoofdlocatie

### Maximum 1 Hoofdlocatie
- Per organisatie, per periode, maximaal 1 hoofdlocatie
- Bij instellen nieuwe hoofdlocatie verliest oude automatisch deze status

## Visuele Indicatoren

### In Locatielijst
- **Groene vinkje**: Locatie heeft Grar ID (op moment van aanmaak gevalideerd)
- **Geen vinkje**: Geen Grar-koppeling (handmatig, KBO, of buiten Vlaanderen)

### Bij Bewerken
- **Groene badge**: "Het gekozen adres is gekend in Grar"
- **Gele badge**: "Het gekozen adres is niet gekend in Grar"

## Beperkingen

**Grar:**
- Alleen Vlaanderen - Brussel, Wallonië en buitenland werken niet
- Geen synchronisatie - Grar ID is momentopname bij aanmaak
- Handmatige wijziging na Grar-selectie verbreekt link

**KBO:**
- Alleen maatschappelijke zetel - andere locaties niet automatisch
- Bij fout adres: correctie moet in KBO gebeuren (FOD Economie)

**Deduplicatie:**
- Alleen exacte match - varianten worden niet herkend
  - "Kunstlaan 16" ≠ "Kunstlaan 16A"
  - "Brussel" ≠ "Brussels"

## Beslissingsmatrix

| Situatie | Actie |
|----------|-------|
| Nieuwe organisatie met KBO | Koppel aan KBO → locatie automatisch |
| Extra locatie toevoegen | Handmatig aanmaken + Grar gebruiken (indien Vlaanderen) |
| Adres buiten Vlaanderen | Volledig handmatig invoeren |
| Meerdere organisaties, zelfde adres | Hergebruik bestaande locatie |
| KBO-adres is fout | Laat corrigeren in KBO, sync gebeurt automatisch |

## Metrics (voor monitoring)

**Data Kwaliteit:**
- % Locaties met Grar-link (target: >70% voor Vlaamse regio)
- % KBO-organisaties met locatie (target: 100%)

**Synchronisatie:**
- Foutpercentage KBO-sync (target: <1%)
- Aantal succesvol gesynchroniseerde organisaties per nacht

**Gebruik:**
- Aantal hergebruikte locaties (hogere score = betere deduplicatie)
- Aantal niet-Grar locaties (trend: meer internationale organisaties of training nodig)

## Veelgestelde Vragen

**Waarom twee bronnen?**
Verschillende doelen: KBO voor automatische officiële adressen, Grar voor kwaliteitsverbetering bij handmatige invoer.

**Moet ik Grar gebruiken?**
Nee, optioneel. Wel aanbevolen voor Vlaamse adressen (validatie + sneller).

**Hoe vaak KBO-sync?**
Nachtelijk, wijzigingen binnen 24u zichtbaar.

**Kan ik KBO-locatie wijzigen?**
Nee, correctie moet in KBO gebeuren (FOD Economie).

**Waarom kan ik mijn adres niet vinden in Grar?**
Adres ligt buiten Vlaanderen, zeer nieuw, of andere schrijfwijze proberen. Vul dan handmatig in.

**Wat bij foutieve duplicaten?**
Systeem voorkomt exacte duplicaten automatisch. Varianten moet je handmatig opruimen.

## Aanbevelingen

1. **Training**: Zorg dat beheerders weten wanneer KBO of Grar te gebruiken
2. **Monitor KBO-sync**: Houd foutpercentage in de gaten
3. **Grar-gebruik stimuleren**: Voor Vlaamse adressen altijd Grar-zoeken gebruiken
4. **Periodieke opruiming**: Check en merge adresvarianten (bv. "Brussel" vs "Brussels")
