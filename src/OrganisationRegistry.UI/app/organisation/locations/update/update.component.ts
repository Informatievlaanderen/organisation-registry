import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { LocationType, LocationTypeService } from 'services/locationtypes';

import {
  OrganisationLocationService,
  UpdateOrganisationLocationRequest
} from 'services/organisationlocations';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationLocationsUpdateOrganisationLocationComponent implements OnInit {
  public form: FormGroup;
  public locationTypes: SelectItem[];
  public location: SearchResult;

  private readonly updateAlerts = new UpdateAlertMessages('Locatie');
  private locationId: string;
  private locationTypeId: string;

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
      locationName: ['', required],
      locationTypeId: [''],
      locationTypeName: [''],
      isMainLocation: [false, Validators.required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let allLocationTypesObservable = this.locationTypeService.getAllUserPermittedLocationTypes();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let organisationLocationId = res[1]['id'];

        Observable.zip(this.organisationLocationService.get(orgId, organisationLocationId), allLocationTypesObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationLocationRequest) {
    this.form.disable();

    this.organisationLocationService.update(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      );
  }

  private setForm(organisationLocation, allLocationTypes) {
    if (organisationLocation) {
      this.form.setValue(organisationLocation);
      this.location = new SearchResult(
        organisationLocation.locationId,
        organisationLocation.locationName);
      // this.locationType = new SearchResult(
      //   organisationLocation.locationTypeId,
      //   organisationLocation.locationTypeName);
    }

    this.locationTypes = allLocationTypes.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Locatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het locatie. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Locatie bijgewerkt!')
        .withMessage('Locatie is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Locatie kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
