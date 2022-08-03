export enum Role {
  UNKNOWN,
  AlgemeenBeheerder,
  VlimpersBeheerder,
  DecentraalBeheerder,
  RegelgevingBeheerder,
  OrgaanBeheerder,
  Developer,
  AutomatedTask,
  CjmBeheerder,
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
      case "cjmBeheerder": //TODO verify string with API
        return Role.CjmBeheerder;
      default:
        return Role.UNKNOWN;
    }
  }
}
