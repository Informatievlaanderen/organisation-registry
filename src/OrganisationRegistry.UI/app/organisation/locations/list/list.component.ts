import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationLocationListItem } from 'services/organisationlocations';

@Component({
  selector: 'ww-organisation-location-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationLocationsListComponent extends BaseListComponent<OrganisationLocationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
