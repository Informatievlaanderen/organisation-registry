import {Component, OnDestroy, OnInit} from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { TogglesService, Toggles } from 'services/toggles';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class TogglesOverviewComponent implements OnInit, OnDestroy {
  public toggles: Toggles;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private togglesService: TogglesService
  ) {}

  ngOnInit() {
    this.subscriptions.push(this.togglesService
      .getAllToggles()
      .subscribe(result => this.toggles = result));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
