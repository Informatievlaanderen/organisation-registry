import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { FormalFrameworkBodyReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-formalframework-body-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class FormalFrameworkBodyListComponent extends BaseListComponent<FormalFrameworkBodyReportListItem> {
}
