import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationCapacityListItem } from 'services/organisationcapacities';

@Component({
  selector: 'ww-organisation-capacity-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationCapacitiesListComponent extends BaseListComponent<OrganisationCapacityListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
