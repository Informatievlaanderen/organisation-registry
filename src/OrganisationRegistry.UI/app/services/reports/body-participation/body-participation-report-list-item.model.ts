export class BodyParticipationReportListItem {
    constructor(
      public organisationId: string = '',
      public organisationName: string = '',
      public bodyId: string = '',
      public bodyName: string = '',
      public isEffective: boolean = false,
      public isEffectiveTranslation: string = '',
      public maleCount: number = 0,
      public femaleCount: number = 0,
      public unknownCount: number = 0,
      public assignedCount: number = 0,
      public unassignedCount: number = 0,
      public totalCount: number = 0,
      public malePercentage: number = 0,
      public femalePercentage: number = 0,
      public unknownPercentage: number = 0,
      public isCompliant: boolean = false,
    ) { }
  }
