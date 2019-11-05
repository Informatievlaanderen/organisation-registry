import { Component, ElementRef, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';

import { AlertService, AlertBuilder } from 'core/alert';
import { CreateAlertMessages, UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { Location, LocationService } from 'services/locations';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class LocationDetailComponent implements OnInit {
  public isEditMode: boolean;
  public form: FormGroup;

  public location: Location;
  public lastCrabLocation = null;
  public formattedAddress: SearchResult;

  private readonly createAlerts = new CreateAlertMessages('Locatie');
  private readonly updateAlerts = new UpdateAlertMessages('Locatie');

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private locationService: LocationService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      crabLocationId: [''],
      formattedAddress: [''],
      street: ['', required],
      zipCode: ['', required],
      city: ['', required],
      country: ['', required]
    });
  }

  ngOnInit() {
    this.location = new Location();
    this.form.setValue(this.location);

    this.route.params.forEach((params: Params) => {
      this.form.disable();

      let id = params['id'];
      this.isEditMode = id !== null && id !== undefined;

      if (this.isEditMode) {
        this.locationService.get(id)
          .finally(() => this.form.enable())
          .subscribe(
            item => {
              if (item) {
                this.form.setValue(item);
                if (item.crabLocationId) {
                  this.lastCrabLocation = this.form.value;
                  this.formattedAddress = new SearchResult(item.crabLocationId, item.formattedAddress);
                }
              }
            },
            error => {
              let alert = this.updateAlerts.loadError;
              this.alertService.setAlert(
                new AlertBuilder()
                  .error(error)
                  .withTitle(alert.title)
                  .withMessage(alert.message)
                  .build());
            });
      } else {
        this.form.enable();
      }
    });
  }

  crabValueChanged(value: string) {
    let fullAddressRegex = /(.*),([\s][0-9]{4})? (.*)/;

    if (!value)
      return;

    let groups = fullAddressRegex.exec(value);
    if (groups && groups.length === 4) {
      this.form.get('street').setValue(groups[1]);
      this.form.get('zipCode').setValue(groups[2]);
      this.form.get('city').setValue(groups[3]);
      this.form.get('country').setValue('België');
      if (groups[2]) { // has a zipcode
        this.lastCrabLocation = this.form.value;
      } else {
        this.form.get('zipCode').markAsTouched();
        this.form.get('crabLocationId').setValue(null);
        this.lastCrabLocation = null;
      }
    } else {
      if (value.indexOf(',') === -1) {
        this.form.get('street').setValue('');
        this.form.get('street').markAsTouched();

        this.form.get('zipCode').setValue('');
        this.form.get('zipCode').markAsTouched();

        this.form.get('city').setValue(value);
        this.form.get('country').setValue('België');

        this.form.get('crabLocationId').setValue(null);
        this.lastCrabLocation = null;
      } else {
        this.form.setValue(new Location());
      }
    }
  }

  onKey() {
    if (!this.lastCrabLocation) return;

    let location = this.form.value;
    if (location.street !== this.lastCrabLocation.street ||
      location.zipCode !== this.lastCrabLocation.zipCode ||
      location.city !== this.lastCrabLocation.city ||
      location.country !== this.lastCrabLocation.country) {
      this.form.get('crabLocationId').setValue(null);
    }
  }

  createOrUpdate(value: Location) {
    let location = new Location().withValues(value);
    if (this.isEditMode) {
      this.update(location);
    } else {
      this.create(location);
    }
  }

  private create(location: Location) {
    this.locationService.create(location)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let locationUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                ['./../', location.id],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            let alert = this.createAlerts.saveSuccess(location);

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle(alert.title)
                .withMessage(alert.message)
                .linkTo(locationUrl)
                .build());
          }
        },
        error => {
          let alert = this.createAlerts.saveError;
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle(alert.title)
              .withMessage(alert.message)
              .build());
        });
  }

  private update(location: Location) {
    this.locationService.update(location)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            let locationUrl = this.router.serializeUrl(
              this.router.createUrlTree(
                ['./../', location.id],
                { relativeTo: this.route }));

            this.router.navigate(['./..'], { relativeTo: this.route });

            let alert = this.updateAlerts.saveSuccess(location);
            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle(alert.title)
                .withMessage(alert.message)
                .linkTo(locationUrl)
                .build());
          }
        },
        error => {
          let alert = this.updateAlerts.saveError;
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle(alert.title)
              .withMessage(alert.message)
              .build());
        });
  }
}
