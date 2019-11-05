import { Input, Component, ChangeDetectionStrategy } from '@angular/core';
import { FormControl } from '@angular/forms';

import { BuildingFilter, BuildingService } from 'services/buildings';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

@Component({
  selector: 'ww-building-autocomplete',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'autocomplete.template.html',
  styleUrls: [ 'autocomplete.style.css' ]
})
export class BuildingAutoComplete {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() initialValue: SearchResult;

  constructor(
    private buildingService: BuildingService
  ) {}

  searchBuildings(search: string): Promise<SearchResult[]> {
    return this.buildingService.search(new BuildingFilter(search))
      .map(pagedResult => pagedResult.data.map(x => new SearchResult(x.id, x.name)))
      .toPromise()
      .catch(this.handleError);
  }

  private handleError(error: any): any {
    console.error('An error occurred', error); // log to console instead
    return new SearchResult('', 'Er heeft zich een fout voorgedaan bij het ophalen van de gegevens.');
  }
}
