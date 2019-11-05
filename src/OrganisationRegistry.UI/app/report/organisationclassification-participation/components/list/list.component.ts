import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Constants } from 'core/constants';
import { BaseListComponent } from 'shared/components/list';

import { OrganisationClassification } from 'services/organisationclassifications';

@Component({
  selector: 'ww-report-classification-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv']
})
export class OrganisationClassificationListComponent extends BaseListComponent<OrganisationClassification> {

  public classificationTag: string;
  public classificationHeading: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {
    super();

    this.route.params.forEach((params: Params) => {
      let tag = params['tag'];
      this.classificationTag = tag;
      this.classificationHeading = this.classificationTag === Constants.PARTICIPATION_MINISTER_TAG ? 'Ministerposten' : 'Beleidsdomeinen';
    });
  }
}
