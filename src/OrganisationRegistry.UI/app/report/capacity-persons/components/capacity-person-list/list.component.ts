import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { CapacityPersonReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-capacity-person-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class CapacityPersonListComponent extends BaseListComponent<CapacityPersonReportListItem> {
}
