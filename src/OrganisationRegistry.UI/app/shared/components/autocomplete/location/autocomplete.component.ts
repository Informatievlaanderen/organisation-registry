import { Input, Component, ChangeDetectionStrategy } from '@angular/core';
import { FormControl } from '@angular/forms';

import {
  LocationService,
  LocationFilter
} from 'services/locations';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

@Component({
  selector: 'ww-location-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: ['autocomplete.style.css']
})
export class LocationAutoComplete {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;

  constructor(
    private locationService: LocationService
  ) {}

  searchLocations(search: string): Promise<SearchResult[]> {
    return this.locationService.search(new LocationFilter(search))
      .map(pagedResult => pagedResult.data.map(x => new SearchResult(x.id, `${x.street}, ${x.zipCode} ${x.city}`)))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error); // log to console instead
    return new SearchResult('', 'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.');
  }
}
