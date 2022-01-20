import { Input, Component, ChangeDetectionStrategy } from '@angular/core';
import { FormControl } from '@angular/forms';

import {
  OrganisationFilter,
  OrganisationService
} from 'services/organisations';

import { SearchResult } from './../../form/form-group-autocomplete';

@Component({
  selector: 'ww-organisation-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: [ 'autocomplete.style.css' ]
})
export class OrganisationAutoComplete {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;
  @Input() excludeId: string;
  @Input() activeOnly: boolean = true;
  @Input() authorizedOnly: boolean = false;

  constructor(
    private organisationService: OrganisationService
  ) {}

  searchOrganisations(search: string): Promise<SearchResult[]> {
    return this.organisationService.search(
      new OrganisationFilter()
        .withName(search)
        .withActiveOnly(this.activeOnly)
        .withAuthorizedOnly(this.authorizedOnly))
      .map(pagedResult => pagedResult.data.map(x => new SearchResult(x.id, `${x.name} ${x.ovoNumber}`)))
      .map(pagedResult => pagedResult.filter(searchResult => !this.excludeId || searchResult.value !== this.excludeId))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error); // log to console instead
    return new SearchResult('', 'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.');
  }
}
