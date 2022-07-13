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
}
