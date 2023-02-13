import {Component, OnDestroy, OnInit, ViewEncapsulation} from '@angular/core';
import {ActivatedRoute, NavigationEnd, NavigationStart, Router} from '@angular/router';
import {Title} from '@angular/platform-browser';

import {AlertService} from './core/alert';
import {ConfigurationService} from "./core/configuration";
import {Environments} from "./environments";
import {Subscription} from "rxjs/Subscription";
import {init as initApm} from '@elastic/apm-rum'

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
    } else if (configurationService.apiUrl.includes('dev-vlaanderen.local')) {
      this.environment = Environments.development;
    } else {
      this.environment = Environments.production;
    }

    if (configurationService.otelServerUri)
      this.initializeApm(router,
        configurationService.otelServerUri,
        configurationService.applicationVersion,
        configurationService.otelDistributedTracingOrigins,
        this.environment);

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
      .subscribe((data: any) => {
        if (data['title']) {
          this.titleService.setTitle('Wegwijs - ' + data['title']);
        } else {
          this.titleService.setTitle('Wegwijs');
        }
        const matomo = (<any>window)._paq;
        if (matomo) {
          matomo.push(['setCustomUrl', (<any>window).location.hash.substr(1)]);
          matomo.push(['setDocumentTitle', data.title]);
          matomo.push(['trackPageView']);
        }
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngOnInit() {
    this.titleService.setTitle('Wegwijs');
  }

  private initializeApm(router, serverUrl, serviceVersion, distributedTracingOrigins, environment) {
    const apm = initApm({

      serviceName: 'OrganisationRegistry_UI',

      serverUrl: serverUrl,

      serviceVersion: serviceVersion,

      // Set the service environment
      environment: environment,
      breakdownMetrics: true,
      distributedTracing: true,
      distributedTracingOrigins: distributedTracingOrigins,
    });

    const spans = {};
    this.subscriptions.push(router.events
      .filter(event => event instanceof NavigationStart)
      .subscribe(navigationStart => {
        let currentTransaction = apm.getCurrentTransaction();
        if (!currentTransaction) {
          return;
        }
        apm.setInitialPageLoadName(navigationStart.url);
        currentTransaction.name = navigationStart.url;
        const span = currentTransaction.startSpan("navigation", "navigation");
        spans[navigationStart.id] = span;
      }));

    this.subscriptions.push(router.events
      .filter(event => event instanceof NavigationEnd)
      .subscribe(navigationEnd => {
        let currentTransaction = apm.getCurrentTransaction();
        if (!currentTransaction) {
          return;
        }
        apm.setInitialPageLoadName(navigationEnd.url);
        spans[navigationEnd.id].end();
        delete spans[navigationEnd.id];
      }));
  }
}
