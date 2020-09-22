import {TerminationStatus} from "./termination-status.enum";

export class OrganisationTermination {
    constructor(
      public organisationId: string,
      public reason: string,
      public code: string,
      public status: TerminationStatus,
      public date: Date,
    ) {

    }
}
