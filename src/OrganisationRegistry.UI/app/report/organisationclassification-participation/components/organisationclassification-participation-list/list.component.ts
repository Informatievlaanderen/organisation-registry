import { Component, Input, Output, EventEmitter } from '@angular/core';

import { DecimalPipe } from '@angular/common';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationClassificationParticipationReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-classification-participation-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class OrganisationClassificationParticipationListComponent extends BaseListComponent<OrganisationClassificationParticipationReportListItem> {

    private lower: number = Math.floor((1 / 3) * 100) / 100;
    private upper: number = Math.ceil((2 / 3) * 100) / 100;

    isMepCompliant(percentage) {
        return percentage >= this.lower && percentage <= this.upper;
    }
}
