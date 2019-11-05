import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Observable } from 'rxjs/Observable';

import { StatsService, Stats } from 'services/stats';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class StatsOverviewComponent implements OnInit {
  public stats: Stats;
  public form: FormGroup;
  public isBusy: Boolean;

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
    this.statsService
      .getAllStats(this.form.value.daysBack)
      .finally(() => this.isBusy = false)
      .subscribe(result => this.stats = result);
  }

  ngOnInit() {
    this.refreshStats();
  }
}
