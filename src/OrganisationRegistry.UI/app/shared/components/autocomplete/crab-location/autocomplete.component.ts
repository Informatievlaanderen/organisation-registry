import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';

import { LocationService } from 'services/locations';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

@Component({
  selector: 'ww-crab-location-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: ['autocomplete.style.css']
})
export class CrabLocationAutoComplete {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;

  @Output() valueChanged = new EventEmitter<any>();

  constructor(
    private locationService: LocationService
  ) {
  }

  onValueChanged(data) {
    this.valueChanged.next(data);
  }

  searchCrabLocations(search: string): Promise<SearchResult[]> {
    return this.locationService.getCrabLocations(search)
      .map(pagedResult => pagedResult.map(x => new SearchResult(x.ID, x.FormattedAddress)))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error); // log to console instead
    return new SearchResult('', 'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.');
  }
}
