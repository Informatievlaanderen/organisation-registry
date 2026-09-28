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
    CjmBeheerder,
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
}
