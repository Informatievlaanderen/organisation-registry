import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationLabelListItem } from 'services/organisationlabels';

@Component({
  selector: 'ww-organisation-label-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationLabelsListComponent extends BaseListComponent<OrganisationLabelListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
