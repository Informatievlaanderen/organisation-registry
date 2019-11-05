export class OrganisationBankAccount {
  constructor(
      public organisationBankAccountId: string,
      public organisationId: string,
      public bankAccountNumber: string,
      public isIban: boolean,
      public validFrom: Date,
      public validTo: Date
    ) {

    }
}
