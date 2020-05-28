export class ParticipationSummaryReportListItem {
    constructor(
      public organisationId: string = '',
      public organisationName: string = '',
      public formalFrameworkId: string = '',
      public formalFrameworkName: string = '',
      public bodyId: string = '',
      public bodyName: string = '',
      public isTotalCompliant: boolean = false,
    ) { }
  }
