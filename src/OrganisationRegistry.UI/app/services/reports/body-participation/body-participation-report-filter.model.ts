export class BodyParticipationReportFilter {
    constructor(
      public entitledToVote: boolean = true,
      public notEntitledToVote: boolean = false
    ) { }
  }
