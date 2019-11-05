import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationOpeningHourListItem } from 'services/organisationopeninghours';

@Component({
  selector: 'ww-organisation-openinghour-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationOpeningHoursListComponent extends BaseListComponent<OrganisationOpeningHourListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
