import {Component, ViewEncapsulation, OnInit, OnDestroy} from '@angular/core';
import {Router, NavigationStart, NavigationEnd, ActivatedRoute} from '@angular/router';
import {Title} from '@angular/platform-browser';

import {AlertService} from './core/alert';
import {ConfigurationService} from "./core/configuration";
import {Environments} from "./environments";
import {Subscription} from "rxjs/Subscription";

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
export class App implements OnInit, OnDestroy {
  public environment: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private titleService: Title,
    private alertService: AlertService,
    private configurationService: ConfigurationService
  ) {

    if (configurationService.apiUrl.includes('dev-vlaanderen.be')) {
      this.environment = Environments.staging;
    }
    else if (configurationService.apiUrl.includes('dev-vlaanderen.local')) {
      this.environment = Environments.development;
    }else {
      this.environment = Environments.production;
    }

    this.subscriptions.push(router.events
      .filter(event => event instanceof NavigationStart)
      .subscribe(() => alertService.clearAlert()));

    this.subscriptions.push(router.events
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
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngOnInit() {
    this.titleService.setTitle('Wegwijs');
  }
}
