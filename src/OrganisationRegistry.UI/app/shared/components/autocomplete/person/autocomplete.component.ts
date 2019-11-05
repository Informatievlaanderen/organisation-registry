import { Input, Component, ChangeDetectionStrategy } from '@angular/core';
import { FormControl } from '@angular/forms';

import {
    PersonFilter,
    PersonService
} from 'services/people';

import { SearchResult } from './../../form/form-group-autocomplete';

@Component({
  selector: 'ww-person-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: ['autocomplete.style.css']
})
export class PersonAutoComplete {

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;

  constructor(
    private personService: PersonService
  ) { }

  searchPeople(search: string): Promise<SearchResult[]> {
    return this.personService.search(new PersonFilter(null, null, search))
      .map(pagedResult => pagedResult.data.map(x => new SearchResult(x.id, x.firstName + ' ' + x.name)))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error); // log to console instead
    return new SearchResult('', 'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.');
  }
}
