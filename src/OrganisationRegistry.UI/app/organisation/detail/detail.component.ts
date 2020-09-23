import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService } from 'core/alert';

import { TogglesService } from 'services/toggles';
import { OidcService } from 'core/auth';

import { OrganisationInfoService } from 'services/organisationinfo';
import { Organisation } from 'services/organisations';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class OrganisationDetailComponent implements OnInit, OnDestroy {
  public organisation: Organisation;
  public enableBankAccounts: Observable<boolean>;
  public enableSync: Observable<boolean>;
  public enableOrganisationRelations: Observable<boolean>;
  public enableOrganisationOpeningHours: Observable<boolean>;

  constructor(
    private route: ActivatedRoute,
    private alertService: AlertService,
    private store: OrganisationInfoService,
    private togglesService: TogglesService,
    private oidcService: OidcService,
  ) {
    this.organisation = new Organisation();
  }

  ngOnInit() {
    this.store
      .organisationChanged
      .subscribe(org => this.organisation = org);

    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.store.loadOrganisation(id);
      this.enableSync = this.oidcService.isLoggedIn && this.oidcService.canEditOrganisation(id);
    });

    this.enableOrganisationRelations = this.togglesService
      .getAllToggles()
      .map(toggles => toggles.enableOrganisationRelations);

    this.enableOrganisationOpeningHours = this.togglesService
      .getAllToggles()
      .map(toggles => toggles.enableOrganisationOpeningHours);

    this.enableBankAccounts = this.oidcService.isLoggedIn;
  }

  ngOnDestroy() {
  }
}
