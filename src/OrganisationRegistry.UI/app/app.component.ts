import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { Router, NavigationStart, NavigationEnd, ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';

import { AlertService } from './core/alert';

@Component({
  selector: 'wegwijs',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    // './../assets/css/vlaanderen-ui.css',
    './app.style.css',
    './shared/components/form/form.style.css'
  ],
  templateUrl: './app.template.html'
})
export class App implements OnInit  {
  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title,
    private alertService: AlertService
  ) {
    router.events
      .filter(event => event instanceof NavigationStart)
      .subscribe(() => alertService.clearAlert());

    router.events
      .filter(event => event instanceof NavigationEnd)
      .map(() => this.activatedRoute)
      .map(route => {
        while (route.firstChild) route = route.firstChild;
        return route;
      })
      .filter(route => route.outlet === 'primary')
      .mergeMap(route => route.data)
      .subscribe(data => {
        if (data['title']) {
          this.titleService.setTitle('Wegwijs - ' + data['title']);
        } else {
          this.titleService.setTitle('Wegwijs');
        }
      });
  }

  ngOnInit() {
    this.titleService.setTitle('Wegwijs');
  }
}
