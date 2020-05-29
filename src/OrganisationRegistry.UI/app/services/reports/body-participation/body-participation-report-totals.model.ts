export class Compliance {
  public static UNKNOWN: string = 'unknown';
  public static NONCOMPLIANT: string = 'nonCompliant';
  public static COMPLIANT: string = 'compliant';
}

export class BodyParticipationReportTotals {

  constructor(
    public maleCount: number = 0,
    public femaleCount: number = 0,
    public unknownCount: number = 0,
    public assignedCount: number = 0,
    public unassignedCount: number = 0,
    public totalCount: number = 0,
    public malePercentage: number = 0,
    public femalePercentage: number = 0,
    public unknownPercentage: number = 0,
    public compliance: string = Compliance.UNKNOWN) {
  }
}
