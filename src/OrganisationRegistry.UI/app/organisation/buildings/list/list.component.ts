import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationBuildingListItem } from 'services/organisationbuildings';

@Component({
  selector: 'ww-organisation-building-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationBuildingsListComponent extends BaseListComponent<OrganisationBuildingListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
