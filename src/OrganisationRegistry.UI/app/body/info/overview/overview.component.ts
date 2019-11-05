import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AlertBuilder, AlertService, Alert, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';
import { AuthService, OidcService } from 'core/auth';

import { Body } from 'services/bodies';
import { BodyInfoService } from 'services/bodyinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyInfoOverviewComponent implements OnInit, OnDestroy {
  public isBusy = true;
  public body: Body;
  public canEditBody: Observable<boolean>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Orgaan');
  private id: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private oidcService: OidcService,
    private store: BodyInfoService
  ) {
    this.body = new Body();
  }

  ngOnInit() {
    let bodyChangedObservable =
      this.store.bodyChanged;

    bodyChangedObservable.subscribe((res) => this.isBusy = false);

    this.subscriptions.push(bodyChangedObservable
      .subscribe(body => {
        if (body) {
          this.body = body;
        }
      }));

    this.canEditBody = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params
      .subscribe(params => {
        this.isBusy = true;
        this.id = params['id'];
        this.canEditBody = this.oidcService.canEditBody(this.id);
        this.store.loadBody(this.id);
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
