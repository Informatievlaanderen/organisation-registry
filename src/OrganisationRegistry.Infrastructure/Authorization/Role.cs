namespace OrganisationRegistry.Infrastructure.Authorization;

public enum Role
{
    AlgemeenBeheerder,
    VlimpersBeheerder,
    DecentraalBeheerder,
    OrgaanBeheerder,
    /// <summary>
    /// Regelgeving en deugdelijk bestuur beheerder
    /// </summary>
    RegelgevingBeheerder,
    Orafin,
    Developer,
    AutomatedTask,

    /// <summary>
    /// VO medewerker: read-only role, conceptually "Publiek + read access to
    /// Functies and Hoedanigheden" (both on organisations and on people).
    /// Holds no write, create or delete rights anywhere, and no scoping
    /// restrictions apply to it (read-only across the whole registry).
    /// See <c>ui-permission-matrix.md</c> for the full rights table.
    /// </summary>
    VoMedewerker,

    /// <summary>
    /// VO Decentraal beheerder: read-only on all screens except
    /// organisation info update (RU), scoped to own OVO and child
    /// organisations. More restricted than <see cref="DecentraalBeheerder"/>
    /// (no write access to sub-resources like contacts, functions, locations,
    /// bodies, etc.) but can still update general organisation fields.
    /// See <c>ui-permission-matrix.md</c> for the full rights table.
    /// </summary>
    VoDecentraalBeheerder,
}
