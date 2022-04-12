import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AlertService} from 'core/alert';
import { PagedEvent, PagedResult} from 'core/pagination';
import { OidcService, Role} from 'core/auth';

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
  public isOrganisationRegistryBeheerder: Observable<boolean>;

  private id: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private oidcService: OidcService,
    public store: OrganisationInfoService,
  ) {
    this.organisation = new Organisation();
    this.children = new PagedResult<OrganisationChild>();
  }

  ngOnInit() {
    let organisationChangedObservable =
      this.store.organisationChanged;

    let childrenChangedObservable =
      this.store.organisationChildrenChanged;

    this.subscriptions.push(
      Observable.zip(organisationChangedObservable, childrenChangedObservable)
      .subscribe((res) => this.isBusy = false));

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

    this.subscriptions.push(this.route.parent.parent.params
      .subscribe(params => {
        this.isBusy = true;
        this.id = params['id'];
        this.isOrganisationRegistryBeheerder = this.oidcService.hasAnyOfRoles([Role.AlgemeenBeheerder]);
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
}
