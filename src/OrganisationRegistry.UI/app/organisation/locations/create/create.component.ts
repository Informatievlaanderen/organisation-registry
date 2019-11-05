import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationLocationRequest,
  OrganisationLocationService
} from 'services/organisationlocations';

import { LocationType, LocationTypeService } from 'services/locationtypes';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationLocationsCreateOrganisationLocationComponent implements OnInit {
  public form: FormGroup;
  public locationTypes: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Locaties');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private locationTypeService: LocationTypeService,
    private organisationLocationService: OrganisationLocationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationLocationId: ['', required],
      organisationId: ['', required],
      locationId: ['', required],
      locationTypeId: [''],
      isMainLocation: [false, Validators.required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationLocationRequest(params['id']));
    });

    this.locationTypeService
      .getAllUserPermittedLocationTypes()
      .finally(() => this.form.enable())
      .subscribe(
        allContactTypes => this.locationTypes = allContactTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Locatie types konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de locatie types. Probeer het later opnieuw.')
              .build()));
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationLocationRequest) {
    this.form.disable();

    this.organisationLocationService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
        if (result) {
          this.router.navigate(['./..'], { relativeTo: this.route });

          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Locatie gekoppeld!')
              .withMessage('Locatie is succesvol gekoppeld.')
              .build());
        }
      },
      error =>
        this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Locatie kon niet gekoppeld worden!')
            .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
            .build())
      );
  }
}
