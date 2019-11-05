import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationClassificationTranslationReportListItem } from 'services/reports';

@Component({
    selector: 'ww-report-classification-organisation-list',
    templateUrl: 'list.template.html',
    styleUrls: ['list.style.css'],
    inputs: ['items', 'isBusy'],
    outputs: ['changePage', 'exportCsv']
})
export class ClassificationOrganisationListComponent extends BaseListComponent<OrganisationClassificationTranslationReportListItem> {
}
