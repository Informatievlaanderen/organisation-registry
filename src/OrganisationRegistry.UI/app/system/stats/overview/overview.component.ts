import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Observable } from 'rxjs/Observable';

import { StatsService, Stats } from 'services/stats';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class StatsOverviewComponent implements OnInit, OnDestroy {
  public stats: Stats;
  public form: FormGroup;
  public isBusy: Boolean;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private formBuilder: FormBuilder,
    private statsService: StatsService
  ) {
    this.form = formBuilder.group({
      daysBack: [7, Validators.nullValidator],
    });
  }

  refreshStats() {
    this.isBusy = true;
    this.subscriptions.push(this.statsService
      .getAllStats(this.form.value.daysBack)
      .finally(() => this.isBusy = false)
      .subscribe(result => this.stats = result));
  }

  ngOnInit() {
    this.refreshStats();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
