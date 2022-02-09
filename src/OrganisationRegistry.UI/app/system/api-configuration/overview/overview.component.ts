import {Component, OnDestroy, OnInit} from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { ApiConfigurationService, ApiConfiguration } from 'services/api-configuration';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class ApiConfigurationOverviewComponent implements OnInit, OnDestroy {
  public apiConfiguration: ApiConfiguration;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private apiConfigurationService: ApiConfigurationService
  ) {}

  ngOnInit() {
    this.subscriptions.push(this.apiConfigurationService
      .getAllToggles()
      .subscribe(result => this.apiConfiguration = result));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
