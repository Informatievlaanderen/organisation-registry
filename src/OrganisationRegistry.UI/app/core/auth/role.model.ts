export enum Role {
  UNKNOWN,
  AlgemeenBeheerder,
  VlimpersBeheerder,
  DecentraalBeheerder,
  RegelgevingBeheerder,
  OrgaanBeheerder,
  Developer,
  AutomatedTask,
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
      case "developer":
        return Role.Developer;
      default:
        return Role.UNKNOWN;
    }
  }
}
