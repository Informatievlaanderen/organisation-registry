import { UUID } from 'angular2-uuid';

export class CreateOrganisationBankAccountRequest {
  public organisationBankAccountId: string = '';
  public organisationId: string = '';
  public bankAccountNumber: string = '';
  public bic: string = '';

  public isIban: Boolean = false;
  public isBic: Boolean = false;
  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationBankAccountId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
