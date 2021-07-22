import { Component, OnInit } from '@angular/core';

import { Constants } from 'core/constants';

@Component({
  templateUrl: 'overview.component.html',
  styleUrls: ['overview.component.css']
})
export class DashboardOverviewComponent implements OnInit {
    get PARTICIPATION_POLICY_DOMAIN_TAG() { return Constants.PARTICIPATION_POLICY_DOMAIN_TAG; }
    get PARTICIPATION_MINISTER_TAG() { return Constants.PARTICIPATION_MINISTER_TAG; }

  constructor() {
  }

  ngOnInit() {

  }
}
