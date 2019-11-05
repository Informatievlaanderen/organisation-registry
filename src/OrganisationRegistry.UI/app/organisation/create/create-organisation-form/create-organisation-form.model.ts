import { UUID } from 'angular2-uuid';
import { ICrudItem } from 'core/crud';

export class CreateOrganisationFormValues implements ICrudItem<CreateOrganisationFormValues> {
  constructor(
    public id: string = UUID.UUID(),
    public name: string = '',
    public shortName: string = '',
    public parentOrganisationId: string = '',
    public description: string = '',
    public purposeIds: Array<string> = [],
    public purposes: Array<string> = [],
    public showOnVlaamseOverheidSites: boolean = false,
    public validFrom: Date = null,
    public validTo: Date = null,
    public kboNumber: string = '',
    public bankAccounts: Array<BankAccount> = [],
    public legalForms: Array<LegalForm> = [],
    public addresses: Array<Address> = []
  ) { }
}

export class BankAccount {
  constructor(
    public iban: string = '',
    public bic: string = '',
    public validFrom: Date = null,
    public validTo: Date = null
  ) {}
}

export class LegalForm {
  constructor(
    public organisationClassificationId: string = '',
    public validFrom: Date = null,
    public validTo: Date = null
  ) {}
}

export class Address {
  constructor(
    public country: string = '',
    public city: string = '',
    public zipCode: string = '',
    public street: string = '',
    public validFrom: Date = null,
    public validTo: Date = null
  ) {}
}
