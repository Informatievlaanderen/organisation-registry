import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import { AddChildAlerts } from './add-child.alerts';

import { CreateOrganisationFormValues } from './create-child-form';

import { OrganisationService } from 'services/organisations';
import { PurposeService } from 'services/purposes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'add-child.template.html',
  styleUrls: ['add-child.style.css']
})
export class OrganisationInfoAddChildOrganisationComponent implements OnInit, OnDestroy {
  public isBusy: boolean = true;
  public organisation;
  public parentOrganisationId: string;
  public purposes: SelectItem[];

  private readonly createAlerts = new AddChildAlerts();

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationService: OrganisationService,
    private purposeService: PurposeService,
    private alertService: AlertService) { }

  ngOnInit() {
    this.organisation = new CreateOrganisationFormValues();

    this.subscriptions.push(this.purposeService
      .getAllPurposes()
      .finally(() => this.isBusy = this.purposes.length <= 0)
      .subscribe(
        allPurposes => {
          this.purposes = allPurposes.map(k => new SelectItem(k.id, k.name));

          this.route.parent.parent.params.forEach((params: Params) => {
            this.parentOrganisationId = params['id'];
            this.organisation = new CreateOrganisationFormValues();
            this.organisation.parentOrganisationId = this.parentOrganisationId;
            this.isBusy = false;
          });
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Beleidsvelden konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de beleidsvelden. Probeer het later opnieuw.')
              .build())));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  create(value: CreateOrganisationFormValues) {
    this.isBusy = true;
    this.subscriptions.push(this.organisationService.create(value)
      .finally(() => this.isBusy = false)
      .subscribe(
        result => this.onCreateSuccess(result, value),
        error => this.alertService.setAlert(this.createAlerts.saveError(error))
      ));
  }

  private onCreateSuccess(result, organisation) {
    if (result) {
      this.router.navigate(['./../'], { relativeTo: this.route });

      let organisationUrl =
        this.router.serializeUrl(
          this.router.createUrlTree(['./../../../', organisation.id], { relativeTo: this.route }));

      this.alertService.setAlert(this.createAlerts.saveSuccess(organisation, organisationUrl));
    }
  }
}
