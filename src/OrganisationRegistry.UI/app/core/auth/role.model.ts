export enum Role {
  UNKNOWN,
  AlgemeenBeheerder,
  VlimpersBeheerder,
  DecentraalBeheerder,
  RegelgevingBeheerder,
  OrgaanBeheerder,
  Developer,
  AutomatedTask,
  VoDecentraalBeheerder,
  VoMedewerker,
}

export class RoleMapping {
  public static map(role: string): Role {
    switch (role) {
      case "algemeenBeheerder":
        return Role.AlgemeenBeheerder;
      case "vlimpersBeheerder":
        return Role.VlimpersBeheerder;
      case "orgaanBeheerder":
        return Role.OrgaanBeheerder;
      case "decentraalBeheerder":
        return Role.DecentraalBeheerder;
      case "regelgevingBeheerder":
        return Role.RegelgevingBeheerder;
      case "voDecentraalBeheerder":
        return Role.VoDecentraalBeheerder;
      case "medewerker":
        return Role.VoMedewerker;
      case "developer":
        return Role.Developer;
      default:
        return Role.UNKNOWN;
    }
  }
}
