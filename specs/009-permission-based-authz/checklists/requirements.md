# Specification Quality Checklist: Permission-Based Authorization

**Feature**: `009-permission-based-authz`
**Spec**: `../spec.md`
**Date**: 2026-08-27
**Clarified**: 2026-08-27

## Content Quality

- [x] No implementation details (specifieke frameworks, klassen, of libraries)
- [x] Focused on user value en business needs (in plaats van technische oplossing)
- [x] Written for non-technical stakeholders (domeintaal, geen code)
- [x] All mandatory sections completed (User Scenarios, Requirements, Success Criteria)

## Requirement Completeness

- [x] Geen openstaande [NEEDS CLARIFICATION] markers (alle 3 opgelost via /speckit.clarify-ronde)
- [x] Requirements zijn testbaar en ondubbelzinnig geformuleerd
- [x] Success criteria zijn meetbaar (percentages, gedragsclaims, meetmethode)
- [x] Success criteria zijn technologie-agnostisch (geen framework/tool namen)
- [x] Alle acceptance scenarios volgen Given/When/Then structuur
- [x] Edge cases geïdentificeerd (onbekende rol, sessie-verlies, conflicterende scopes, cache-invalidatie, migratiepad)
- [x] Scope duidelijk afgebakend
- [x] Assumpties expliciet

## Feature Readiness

- [x] Alle functional requirements (FR-001..FR-015) hebben duidelijke acceptance criteria via user stories en success criteria
- [x] User stories zijn geprioriteerd (P1, P2, P3) en onafhankelijk testbaar
- [x] P1 levert een MVP dat op zichzelf waarde heeft
- [x] Success criteria mappen op user stories: SC-001↔US1, SC-002↔US2, SC-003↔US3
- [x] Geen implementation leakage in requirements

## Resolved Clarifications

1. **Migratiepad** (Edge Cases): **Cutover** — één release. Geen overgangsperiode met dubbele checks. Bij deployment verdwijnen alle interne rol-checks tegelijk.
2. **Auditlogging** (FR-013): **Buiten scope**. Bestaande logging blijft ongewijzigd; geen uitbreiding als onderdeel van deze feature.
3. **Rollen in cache** (FR-015): **Rollen verdwijnen intern volledig.** ACM/IDM blijft rollen aanleveren als authenticatie-input, maar de eigen code zet ze onmiddellijk om naar permissies. Het gecachte security-object bevat na de vertaalstap enkel nog permissies en scope-informatie.

## Notes

- Domeintaal (NL): Nederlandse rolnamen (`algemeenbeheerder`, `decentraalbeheerder`, `Vlimpers`) conform bestaand codebase.
- Permissie-identifiers Engels (`canEditChildren`, ...) conform code-conventie.
- Concrete rol→permissie mapping-tabel is bewust NIET in deze spec vastgelegd; hoort in de planfase.
- Cutover-keuze impliceert dat er tijdens de plan-fase een volledige inventaris moet komen van alle huidige rol-checks (controllers + policies + security-helpers), zodat niets over het hoofd gezien wordt bij de eenmalige omzetting.
