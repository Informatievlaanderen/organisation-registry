# Vragen voor ACM/IDM

Gegenereerd op basis van de BFF-demo implementatie (RFC 8693 token exchange via Keycloak,
OAuth2 introspection door de API). De vragen hieronder zijn de gaten die we niet kunnen
invullen zonder informatie van ACM/IDM.

---

## 1. Welke claims zitten in de introspection response?

In de BFF-flow doet de API een OAuth2 introspection call op het access token van de
eindgebruiker. We veronderstellen dat de introspection response de volgende claims bevat:

- `given_name` / `family_name`
- `vo_id` (gebruikersidentificatie)
- `iv_wegwijs_rol_3D` (rollen, één of meerdere waarden)

**Vraag**: Bevestig welke claims de introspection response precies bevat in productie, en
of de claim-namen overeenkomen met wat in de OIDC `userinfo` of het access token zit.
Zijn er claims die *alleen* in het access token zitten maar *niet* in de introspection
response?

---

## 2. Token exchange (RFC 8693): is dit ondersteund in productie?

In de demo gebruiken we Keycloak 26 met RFC 8693 token exchange: de Nuxt BFF wisselt het
access token van de eindgebruiker in voor een token dat de API kan accepteren (subject
token → actor token). In de demo werkt dit omdat we zelf Keycloak beheren.

**Vraag**: Ondersteunt ACM/IDM token exchange (RFC 8693 `urn:ietf:params:oauth:grant-type:token-exchange`)
in productie? Zo ja, welke beperkingen gelden er (audience, scope, client-rechten)?
Zo nee, wat is het aanbevolen alternatief voor een BFF-patroon?

---

## 3. Introspection: aparte client of dezelfde als de EditApi client?

In de demo hergebruiken we de `EditApi` client credentials voor de introspection call
(de API valideert het BFF-token door introspection met de eigen client). Dit is
pragmatisch maar mogelijk niet de bedoeling.

**Vraag**: Moet de API een aparte client registreren voor introspection, of mag hij zijn
eigen `EditApi` client hergebruiken? Zijn er beperkingen op wie introspection mag aanroepen?

---

## 4. Rollen via introspection: `iv_wegwijs_rol_3D`

We verwachten dat `iv_wegwijs_rol_3D` beschikbaar is in de introspection response, net
zoals in het JWT access token. In de demo werkt dit via Keycloak claim-mapping.

**Vraag**: Garandeert ACM/IDM dat `iv_wegwijs_rol_3D` ook in de introspection response
zit? Of zit die claim alleen in het access token zelf (JWT)? Indien het token een opaque
token is, hoe worden rollen dan meegestuurd?

---

## 5. Opaque tokens vs. JWT access tokens

In de demo gebruikt Keycloak standaard JWT access tokens die de API ook direct kan
valideren. In productie is het onduidelijk of ACM/IDM opaque of JWT tokens uitgeeft.

**Vraag**: Geeft ACM/IDM opaque of JWT access tokens uit? Als het JWT tokens zijn: mag
de API ze direct valideren (via JWKS), of is introspection de aanbevolen route? Heeft
dit invloed op de BFF-aanpak?

---

## 6. `Developer`-rol: bestaat die in productie?

In de bestaande JWT-flow kent de API een `Developer`-rol die toegekend wordt op basis
van een lijst van `vo_id` waarden in de configuratie. Deze rol ontbreekt in de nieuwe
`BffClaimsTransformation`.

**Vraag**: Bestaat de `Developer`-rol in de productie ACM/IDM configuratie, of is dit
puur een lokale dev-mechanisme? Moeten BFF-gebruikers ooit `Developer`-rechten kunnen
krijgen?

---

## 7. `DecentraalBeheerder`: formaat van de rol met OVO-nummer

De `DecentraalBeheerder` rol bevat een OVO-nummer in het formaat:
`wegwijsbeheerder-decentraalbeheerder:OVO001234`

We parsen dit als: strip `wegwijsbeheerder-`, split op `:`, neem index `[1]`.

**Vraag**: Is dit het correcte en stabiele formaat in productie? Kunnen er meerdere
OVO-nummers in één rol-waarde zitten, of altijd één per claim-waarde?
