import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import {
  OrganisationBuildingService,
  CreateOrganisationBuildingRequest
} from 'services/organisationbuildings';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationBuildingsCreateOrganisationBuildingComponent implements OnInit, OnDestroy {
  public form: FormGroup;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationBuildingService: OrganisationBuildingService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationBuildingId: ['', required],
      organisationId: ['', required],
      buildingId: ['', required],
      isMainBuilding: [false, Validators.required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationBuildingRequest(params['id']));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationBuildingRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationBuildingService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
        if (result) {
          this.router.navigate(['./..'], { relativeTo: this.route });

          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Gebouw gekoppeld!')
              .withMessage('Gebouw is succesvol gekoppeld.')
              .build());
        }
      },
      error =>
        this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Gebouw kon niet gekoppeld worden!')
            .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
            .build())
      ));
  }
}
