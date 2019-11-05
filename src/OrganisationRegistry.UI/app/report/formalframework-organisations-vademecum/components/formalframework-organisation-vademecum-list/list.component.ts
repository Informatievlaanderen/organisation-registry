import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { FormalFrameworkOrganisationReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-formalframework-organisation-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class FormalFrameworkOrganisationVademecumListComponent extends BaseListComponent<FormalFrameworkOrganisationReportListItem> {
}
