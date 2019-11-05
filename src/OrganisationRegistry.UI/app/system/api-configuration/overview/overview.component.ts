import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { ApiConfigurationService, ApiConfiguration } from 'services/api-configuration';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ApiConfigurationOverviewComponent implements OnInit {
  public apiConfiguration: ApiConfiguration;

  constructor(
    private apiConfigurationService: ApiConfigurationService
  ) {}

  ngOnInit() {
    this.apiConfigurationService
      .getAllToggles()
      .subscribe(result => this.apiConfiguration = result);
  }
}
