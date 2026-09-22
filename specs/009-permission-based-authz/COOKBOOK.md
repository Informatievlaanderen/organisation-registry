# Cookbook: permission-based authz voor een nieuwe resource

> Doel: stap-voor-stap recept om een resource (bijv. labels, capacities, classifications) om te bouwen naar het Model C-pattern dat we gisteren voor keys hebben gelegd.
> Uitgangspunt: je hebt al een resource met een command controller, handlers, policy en tests. Die willen we omzetten naar `Permission` + `IRestriction`.

---

## 0. Voor je begint

Bekijk eerst hoe keys het nu doen. De gouden referentie:

- `src/OrganisationRegistry/Handling/Authorization/KeyPolicy.cs`
- `src/OrganisationRegistry/Organisation/Keys/AddOrganisationKeyCommandHandler.cs`
- `src/OrganisationRegistry/Organisation/Keys/UpdateOrganisationKeyCommandHandler.cs`
- `src/OrganisationRegistry.Api/Backoffice/Organisation/Key/OrganisationKeyController.cs`
- `src/OrganisationRegistry.Api/Backoffice/Parameters/KeyType/KeyTypeController.cs`
- `src/OrganisationRegistry.Api/Backoffice/Organisation/Key/OrganisationKeyCommandController.cs`
- `test/OrganisationRegistry.UnitTests/SecurityPolicy/KeyPolicyTests.cs`

---

## 1. Bepaal het permission

Vraag jezelf af: welke actie doet de gebruiker?

| Resource | Actie | Permission |
|---|---|---|
| Keys | beheren | `CanManageKeys` |
| Labels op organisatie | bewerken | `CanEditOrganisationLabels` |
| Capacities | beheren | `CanManageCapacities` |
| Classifications | beheren | `CanManageOrganisationClassifications` |

> Bestaande permissies vind je in `src/OrganisationRegistry.Infrastructure/Authorization/Permission.cs`. Voeg alleen een nieuwe toe als er echt geen passende bestaat.

---

## 2. Map het permission in `RolePermissionMap` en `ScopePermissionMap`

### 2.1 Rollen (`RolePermissionMap.cs`)

Zet het permission in de juiste rollen. Voorbeeld voor labels:

```csharp
[Role.AlgemeenBeheerder] = PermissionSet.Of(
    // ... andere permissies
    Permission.CanEditOrganisationLabels),

[Role.CjmBeheerder] = PermissionSet.Of(
    Permission.CanAddBodies,
    Permission.CanEditBodies,
    Permission.CanEditOrganisationLabels),

[Role.VlimpersBeheerder] = PermissionSet.Of(
    Permission.CanEditVlimpers,
    Permission.CanEditChildren,
    Permission.CanEditOrganisationLabels),

[Role.DecentraalBeheerder] = PermissionSet.Of(
    Permission.CanEditChildren,
    // ...
    Permission.CanEditOrganisationLabels),
```

### 2.2 Scopes (`ScopePermissionMap.cs`)

Als een M2M-client deze actie moet kunnen doen, voeg het toe aan de bijbehorende scope:

```csharp
[AcmIdmConstants.Scopes.TestClient] = PermissionSet.Of(
    // ...
    Permission.CanEditOrganisationLabels),
```

> CJM en Orafin krijgen **niet** zomaar extra permissies. Alleen als de productafspraak dat zegt.

### 2.3 Test de mapping

Voeg een test toe in:

- `test/OrganisationRegistry.UnitTests/Authorization/RolePermissionMapTests.cs`
- `test/OrganisationRegistry.UnitTests/Authorization/ScopePermissionMapTests.cs`

Voorbeeld:

```csharp
[Theory]
[InlineData(Role.AlgemeenBeheerder, Permission.CanEditOrganisationLabels)]
[InlineData(Role.CjmBeheerder, Permission.CanEditOrganisationLabels)]
[InlineData(Role.VlimpersBeheerder, Permission.CanEditOrganisationLabels)]
[InlineData(Role.DecentraalBeheerder, Permission.CanEditOrganisationLabels)]
[InlineData(Role.Developer, Permission.CanEditOrganisationLabels)]
public void Roles_that_edit_labels_grant_CanEditOrganisationLabels(Role role, Permission permission)
{
    RolePermissionMap.For(role).Contains(permission).Should().BeTrue();
}
```

---

## 3. Definieer de context (restriction context)

De policy moet weten **waarover** de beslissing gaat. Voor keys is dat `KeyContext`:

```csharp
public sealed record KeyContext(bool IsUnderVlimpersManagement, Guid[] KeyTypeIds)
    : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => KeyTypeIds;
}
```

Voor labels zou je iets soortgelijks maken:

```csharp
public sealed record LabelContext(
    bool IsUnderVlimpersManagement,
    string OvoNumber,
    Guid[] LabelTypeIds)
    : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => LabelTypeIds;
}
```

Plaats deze in:

```
src/OrganisationRegistry.Infrastructure/Authorization/Restrictions/
```

---

## 4. Bouw de restricties

Kijk welke beperkingen van toepassing zijn.

### 4.1 Allow-list

Voor Vlimpers-beheerders die alleen bepaalde types mogen gebruiken:

```csharp
public static class LabelRestrictions
{
    public static IRestriction VlimpersManaged(IEnumerable<Guid> allowedLabelTypeIds)
        => new CompositeAndRestriction(
            new RequireUnderVlimpersManagementRestriction(),
            new AllowListRestriction<LabelContext>(allowedLabelTypeIds));
}
```

### 4.2 Organisation ownership via `UserContext`

Als `DecentraalBeheerder` alleen labels mag bewerken van "zijn" organisaties, dan is dat **user-based**. We hebben daarvoor een `UserContext` toegevoegd die de `IUser` meedraagt:

```csharp
public sealed record UserContext(IUser User) : IRestrictionContext
{
    public IEnumerable<Guid> RelevantIds => Enumerable.Empty<Guid>();
}
```

Een ownership-restrictie ziet er zo uit:

```csharp
public sealed class DecentraalBeheerderForOrganisationRestriction : IRestriction
{
    private readonly Guid _organisationId;

    public DecentraalBeheerderForOrganisationRestriction(Guid organisationId)
        => _organisationId = organisationId;

    public bool IsOkWith(params IRestrictionContext[] contexts)
    {
        var userContext = contexts.OfType<UserContext>().FirstOrDefault();
        if (userContext is null)
            return false;

        return userContext.User.IsDecentraalBeheerderForOrganisation(_organisationId);
    }
}
```

> Het equivalent van `ISecurityService.HasPermissionsForOrganisation` zit al in `IUser`. Je hoeft dus geen `ISecurityService` aan te roepen vanuit de restrictie.

---

## 5. Schrijf de policy

Vervang de role-checks door `IsSatisfiedFor`.

### Oud (`LabelPolicy` nu)

```csharp
public AuthorizationResult Check(IUser user)
{
    if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
        return AuthorizationResult.Success();

    if (_isUnderVlimpersManagement &&
        user.IsInAnyOf(Role.VlimpersBeheerder) && AreAllLabelsofTypeVlimpers(_labelTypeIds))
        return AuthorizationResult.Success();

    if (!user.IsDecentraalBeheerderForOrganisation(_ovoNumber))
        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    if (_isUnderVlimpersManagement && AreAnyLabelsofTypeVlimpers(_labelTypeIds))
        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    return AuthorizationResult.Success();
}
```

### Nieuw (Model C)

```csharp
public class LabelPolicy : ISecurityPolicy
{
    private readonly LabelContext _context;

    public LabelPolicy(bool isUnderVlimpersManagement, string ovoNumber, params Guid[] labelTypeIds)
    {
        _context = new LabelContext(isUnderVlimpersManagement, ovoNumber, labelTypeIds);
    }

    public AuthorizationResult Check(IUser user)
    {
        // Stap 1: permission + config-based restrictions (Vlimpers allow-list)
        if (!user.IsSatisfiedFor(Permission.CanEditOrganisationLabels, _context))
            return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

        // Stap 2: user-based ownership rule voor DecentraalBeheerder
        if (user.IsInAnyOf(Role.DecentraalBeheerder) &&
            !user.IsDecentraalBeheerderForOrganisation(_context.OvoNumber))
            return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

        return AuthorizationResult.Success();
    }

    public override string ToString()
        => "Geen machtiging op labeltype.";
}
```

De config-based complexiteit verhuist naar `RolePermissionMap.RestrictedGrantsFor`:

```csharp
Role.VlimpersBeheerder => PermissionSet.Of(
    Permission.CanEditOrganisationLabels.RestrictedTo(
        LabelRestrictions.VlimpersManaged(
            configuration.Authorization.LabelIdsAllowedForVlimpers))),
```

### 5.1 Waarom de DecentraalBeheerder-eigenaarsregel apart blijft

De meeste restrictions zijn **config-based** ("dit labeltype mag voor Vlimpers"). Die passen in `IRestriction`.

De DecentraalBeheerder-regel is **user-based**: "deze gebruiker mag alleen organisaties bewerken die in zijn eigen lijst staan". Die lijst staat in `IUser.OrganisationIds`.

`IRestriction.IsOkWith(IRestrictionContext context)` krijgt **geen** `IUser` mee. Je kunt dus niet zomaar in een restriction checken of de gebruiker een bepaalde organisatie mag bewerken.

**Oplossing:** houd de ownership-check in de policy. Het equivalent van `ISecurityService.HasPermissionsForOrganisation` zit al in `IUser`:

```csharp
public bool IsDecentraalBeheerderForOrganisation(Guid organisationId)
    => IsInAnyOf(Role.DecentraalBeheerder) &&
       OrganisationIds.Contains(organisationId);
```

Je hoeft dus **geen** `ISecurityService` aan te roepen vanuit de policy; `IUser` heeft de data al.

> Dit is geen nederlaag voor het permission-model. De controller gate checkt permissions; de policy checkt resource-specifieke regels. Ownership is een resource-specifieke regel die nu eenmaal per user verschilt.

---

## 6. Update de command controller

Zet `[OrganisationRegistryAuthorize]` op permission in plaats van role.

### Voorbeeld labels (al gedaan)

```csharp
[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanEditOrganisationLabels })]
public class OrganisationLabelCommandController : OrganisationRegistryCommandController
```

Als je dit bij een andere resource doet, vervang dan dit soort oude constructs:

```csharp
// OUD
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.CjmBeheerder)]

// NIEUW
[OrganisationRegistryAuthorize(RequiredPermissions = new[] { Permission.CanEditOrganisationLabels })]
```

---

## 7. Update de command handlers

De handler moet de policy toepassen op het aggregate-niveau.

### Voorbeeld labels (al gedaan)

```csharp
public Task Handle(ICommandEnvelope<AddOrganisationLabel> envelope)
    => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
        .WithLabelPolicy(_organisationRegistryConfiguration, envelope.Command)
        .Handle(session => { ... });
```

Als je de policy refactort, controleer dan dat `WithLabelPolicy` de nieuwe `LabelPolicy` aanmaakt met de juiste context.

---

## 8. Update de read controllers

Read controllers geven vaak keuzelijsten terug. Die moeten filteren op wat de gebruiker mag zien/selecteren.

### Voorbeeld `OrganisationLabelController`

```csharp
var user = await securityService.GetUser(User);
Func<Guid, bool> isAuthorizedForLabelType = labelTypeId =>
    new LabelPolicy(
        memoryCaches.UnderVlimpersManagement.Contains(organisationId),
        memoryCaches.OvoNumbers[organisationId],
        labelTypeId)
    .Check(user)
    .IsSuccessful;

var pagedOrganisations = new OrganisationLabelListQuery(
    context,
    organisationId,
    isAuthorizedForLabelType).Fetch(filtering, sorting, pagination);
```

### Voorbeeld `LabelTypeController`

```csharp
Func<Guid, bool> isAuthorizedForLabelType = labelTypeId =>
    !forOrganisationId.HasValue ||
    new LabelPolicy(
        memoryCaches.UnderVlimpersManagement.Contains(forOrganisationId.Value),
        memoryCaches.OvoNumbers[forOrganisationId.Value],
        labelTypeId)
    .Check(user)
    .IsSuccessful;
```

---

## 9. Voeg `CanSelect` / `CanEdit` toe aan de response (optioneel)

Als de UI moet weten of een item selecteerbaar/bewerkbaar is, voeg dan expliciete booleans toe aan de query result.

### Voorbeeld key responses

```csharp
// OrganisationKeyListQueryResult
public bool IsEditable { get; }
public bool CanEdit { get; }   // nieuw, zelfde waarde

// KeyTypeListItemResult
public bool UserPermitted { get; }
public bool CanSelect { get; } // nieuw, zelfde waarde
```

Doe dit alleen als de UI erom vraagt. Verwijder de oude namen niet — backward compatibility.

---

## 10. Tests

### 10.1 Unit tests voor de policy

Locatie: `test/OrganisationRegistry.UnitTests/SecurityPolicy/<Resource>PolicyTests.cs`

Test scenario's:

- Gebruiker zonder permission → fail.
- `AlgemeenBeheerder` → success.
- `CjmBeheerder` → success (als dat de afspraak is).
- `VlimpersBeheerder` + Vlimpers-managed org + allowed type → success.
- `VlimpersBeheerder` + niet-Vlimpers org → fail.
- `VlimpersBeheerder` + niet-allowed type → fail.
- `DecentraalBeheerder` + eigen org → success / fail afhankelijk van regels.

### 10.2 Unit tests voor de mapping

Zie stap 2.3.

### 10.3 Unit tests voor restrictions

Voeg tests toe in `test/OrganisationRegistry.UnitTests/Authorization/Restrictions/`:

- `AllowListRestrictionTests`
- `CompositeAndRestrictionTests`
- `<Resource>ContextTests` (indien logisch)

### 10.4 Integration tests

- API controller geeft 403 zonder permission.
- API controller geeft 400/201 met juiste permission.
- M2M scopes die het permission niet hebben, falen op de controller gate.
- Read endpoints filteren correct.

Locaties:

- `test/OrganisationRegistry.Api.IntegrationTests/Security/ControllerPermissionEnforcementTests.cs`
- `test/OrganisationRegistry.Api.IntegrationTests/EditApi/CreateOrUpdate<Resource>Tests.cs`
- `test/OrganisationRegistry.Api.IntegrationTests/BackOffice/Organisation/<Resource>Tests.cs`

### 10.5 Run alle suites

```bash
dotnet test test/OrganisationRegistry.UnitTests/OrganisationRegistry.UnitTests.csproj
dotnet test test/OrganisationRegistry.Api.IntegrationTests/OrganisationRegistry.Api.IntegrationTests.csproj
dotnet test test/OrganisationRegistry.SqlServer.IntegrationTests/OrganisationRegistry.SqlServer.IntegrationTests.csproj
```

---

## 11. Controlelijst voorafgaand aan commit

- [ ] Permission bestaat of is toegevoegd.
- [ ] `RolePermissionMap` bijgewerkt.
- [ ] `ScopePermissionMap` bijgewerkt (indien M2M).
- [ ] Policy gebruikt `IsSatisfiedFor` in plaats van `IsInAnyOf(Role...)`.
- [ ] Restriction context + restricties aangemaakt.
- [ ] Command controller heeft `RequiredPermissions`.
- [ ] Command handler roept de juiste `With...Policy` aan.
- [ ] Read controllers filteren met de policy.
- [ ] Response DTO's hebben indien nodig `CanSelect`/`CanEdit`.
- [ ] Unit tests voor policy groen.
- [ ] Unit tests voor mapping groen.
- [ ] Unit tests voor restrictions groen.
- [ ] Integration tests groen.
- [ ] Geen secrets in commits (`detect-secrets scan`).

---

## 12. Veelvoorkomende valkuilen

| Fout | Oplossing |
|---|---|
| Permission bestaat maar is niet gemapt | Altijd zowel `RolePermissionMap` als `ScopePermissionMap` checken. |
| Policy checkt nog rollen | Vervang door `user.IsSatisfiedFor(permission, context)`. |
| Restriction krijgt geen user mee | Stop user-afhankelijke data in de context of vraag om hulp. |
| Controller laat iedereen door | Vergeet niet `RequiredPermissions` te zetten; leeg = geen check. |
| Read controller toont alles | Bouw de `Func<Guid, bool>` en geef mee aan de query. |
| Test scope mist permission | `TestClient` scope moet het permission bevatten, anders falen integratietests. |
| `[Theory]` + `[SkipBankAccounts]` faalt | Gebruik `[Theory(Skip = "...")]` in plaats van een skip-attribute. |

---

## 13. Samenvatting: flow voor labels

```text
1. Permission CanEditOrganisationLabels is al mapped
2. Refactor LabelPolicy naar IsSatisfiedFor + LabelContext
3. Voeg LabelRestrictions toe voor Vlimpers allow-list
4. Bewaar DecentraalBeheerder-eigenaarsregel (restrictie of policy-hulp)
5. Controllers gebruiken al RequiredPermissions = CanEditOrganisationLabels
6. Read controllers filteren al met LabelPolicy
7. Update handlers gebruiken al WithLabelPolicy
8. Schrijf/vervang unit tests voor LabelPolicy
9. Draai alle suites
```