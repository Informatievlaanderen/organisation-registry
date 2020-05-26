import { Component, Input, Output, EventEmitter } from '@angular/core';

import { DecimalPipe } from '@angular/common';

import { BaseListComponent } from 'shared/components/list';

import { ParticipationSummaryReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-participation-summaries-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class ParticipationSummaryListComponent extends BaseListComponent<ParticipationSummaryReportListItem> {
}
