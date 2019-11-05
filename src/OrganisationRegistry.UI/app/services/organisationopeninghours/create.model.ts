import { UUID } from 'angular2-uuid';

export class CreateOrganisationOpeningHourRequest {
  public organisationOpeningHourId: string = '';
  public organisationId: string = '';

  public opens: string = '';
  public closes: string = '';
  public dayOfWeek: Number = null;

  public validFrom: Date = null;
  public validTo: Date = null;

  constructor(organisationId) {
    this.organisationOpeningHourId = UUID.UUID();
    this.organisationId = organisationId;
  }
}
