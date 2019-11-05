export class OrganisationBankAccountListItem {
  constructor(
    public organisationBankAccountId: string = '',
    public bankAccountNumber: string = '',
    public isIban: boolean,
    public bic: string = '',
    public isBic: boolean,
    public validFrom: Date,
    public validTo: Date,
    public isActive: boolean,
    public isEditable: boolean,
  ) { }
}
