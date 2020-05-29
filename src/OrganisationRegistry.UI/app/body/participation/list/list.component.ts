import { Input, Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import {
  BodyParticipationReportListItem,
  BodyParticipationReportTotals, Compliance
} from 'services/reports';

@Component({
  selector: 'ww-body-participation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyParticipationListComponent extends BaseListComponent<BodyParticipationReportListItem> {

  @Input('totals') totals: BodyParticipationReportTotals;
  @Input('showTotals') showTotals: true;

  private lower: number = Math.floor((1 / 3) * 100) / 100;
  private upper: number = Math.ceil((2 / 3) * 100) / 100;

  isMepCompliant(compliance) {
    return compliance === Compliance.COMPLIANT;
  }

  isNonMepCompliant(compliance) {
    return compliance === Compliance.NONCOMPLIANT;
  }
}
