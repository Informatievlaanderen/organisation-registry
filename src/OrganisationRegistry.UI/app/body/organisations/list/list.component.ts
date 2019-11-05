import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyOrganisationListItem } from 'services/bodyorganisations';

@Component({
  selector: 'ww-body-organisation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyOrganisationsListComponent extends BaseListComponent<BodyOrganisationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
