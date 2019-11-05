import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { BuildingOrganisationReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-building-organisation-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class BuildingOrganisationListComponent extends BaseListComponent<BuildingOrganisationReportListItem> {
}
