import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { Constants } from 'core/constants';

import {
  TogglesService,
  Toggles
} from 'services/toggles';

@Component({
  templateUrl: 'overview.component.html',
  styleUrls: ['overview.component.css']
})
export class DashboardOverviewComponent implements OnInit {
    public enableVademecumParticipationReporting: Observable<boolean>;
    public enableFormalFrameworkBodiesReporting: Observable<boolean>;

    get PARTICIPATION_POLICY_DOMAIN_TAG() { return Constants.PARTICIPATION_POLICY_DOMAIN_TAG; }
    get PARTICIPATION_MINISTER_TAG() { return Constants.PARTICIPATION_MINISTER_TAG; }

  constructor(private togglesService: TogglesService) {
    this.enableVademecumParticipationReporting = this.togglesService
      .getAllToggles()
      .map(toggles => toggles.enableVademecumParticipationReporting);
    this.enableFormalFrameworkBodiesReporting = this.togglesService
      .getAllToggles()
      .map(toggles => toggles.enableFormalFrameworkBodiesReporting);
  }

  ngOnInit() {

  }
}
