import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationRegulationListItem } from 'services/organisationregulations';

@Component({
  selector: 'ww-organisation-regulation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationRegulationsListComponent extends BaseListComponent<OrganisationRegulationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
