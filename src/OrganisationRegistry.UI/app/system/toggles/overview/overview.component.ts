import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { TogglesService, Toggles } from 'services/toggles';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class TogglesOverviewComponent implements OnInit {
  public toggles: Toggles;

  constructor(
    private togglesService: TogglesService
  ) {}

  ngOnInit() {
    this.togglesService
      .getAllToggles()
      .subscribe(result => this.toggles = result);
  }
}
