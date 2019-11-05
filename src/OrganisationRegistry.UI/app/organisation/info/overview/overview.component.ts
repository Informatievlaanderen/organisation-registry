import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { combineLatest } from 'rxjs/observable/combineLatest';

import { AlertBuilder, AlertService, Alert, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';
import { AuthService, OidcService } from 'core/auth';

import { OrganisationChild, Organisation } from 'services/organisations';
import { OrganisationInfoService } from 'services/organisationinfo';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationInfoOverviewComponent implements OnInit, OnDestroy {
  public isBusy = true;
  public children: PagedResult<OrganisationChild> = new PagedResult<Organisation>();
  public organisation: Organisation;
  public canEditOrganisation: Observable<boolean>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie');
  private id: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();
  private organisationChildrenChangedSubscription: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private oidcService: OidcService,
    private store: OrganisationInfoService
  ) {
    this.organisation = new Organisation();
    this.children = new PagedResult<OrganisationChild>();
  }

  ngOnInit() {
    let organisationChangedObservable =
      this.store.organisationChanged;

    let childrenChangedObservable =
      this.store.organisationChildrenChanged;

    Observable.zip(organisationChangedObservable, childrenChangedObservable)
      .subscribe((res) => this.isBusy = false);

    this.subscriptions.push(organisationChangedObservable
      .subscribe(organisation => {
        if (organisation) {
          this.organisation = organisation;
        }
      }));

    this.subscriptions.push(childrenChangedObservable
      .subscribe(children => {
        if (children) {
          this.children = children;
        }
      }));

    this.canEditOrganisation = Observable.of(false);
    this.subscriptions.push(this.route.parent.parent.params
      .subscribe(params => {
        this.isBusy = true;
        this.id = params['id'];
        this.canEditOrganisation = this.oidcService.canEditOrganisation(this.id);
        this.store.loadOrganisation(this.id);
        this.store.loadChildren(this.id);
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    return this.store.changePage(this.id, event);
  }

  get canCoupleKbo(){
    return combineLatest(
      this.oidcService.canEditOrganisation(this.id),
      this.store.organisationChanged,
      (canEditOrg, org) => {
        console.log('canCouple', canEditOrg, org.kboNumber);
        return canEditOrg && !org.kboNumber
      });
  }
}
