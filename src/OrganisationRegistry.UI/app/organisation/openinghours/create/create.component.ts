import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationOpeningHourRequest,
  OrganisationOpeningHourService
} from 'services/organisationopeninghours';

import {
  ManagementService
} from 'services/management';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationOpeningHoursCreateOrganisationOpeningHourComponent implements OnInit {
  public form: FormGroup;
  public days: SelectItem[];
  public hours: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Benaming');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationOpeningHourService: OrganisationOpeningHourService,
    private managementService: ManagementService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationOpeningHourId: ['', required],
      organisationId: ['', required],
      opens: ['', required],
      closes: ['', required],
      dayOfWeek: [''],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    // this.form.disable();

    // this.route.parent.parent.params.forEach((params: Params) => {
    //   this.form.setValue(new CreateOrganisationOpeningHourRequest(params['id']));
    // });

    // this.form.enable();
    let allDaysObservable = this.managementService.getAllDays();
    let allHoursObservable = this.managementService.getAllHours();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];

        Observable.zip(
          allDaysObservable,
          allHoursObservable)
          .subscribe(
            item => this.setForm(new CreateOrganisationOpeningHourRequest(orgId), item[0], item[1]),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationOpeningHourRequest) {
    this.form.disable();

    this.organisationOpeningHourService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Benaming aangemaakt!')
                .withMessage('Benaming is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Benaming kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }

  private setForm(organisationOpeningHour, allDays, allHours) {
    if (organisationOpeningHour) {
      this.form.setValue(organisationOpeningHour);
    }

    this.days = allDays.map(k => new SelectItem(k.dayOfWeek, k.label));
    this.hours = allHours.map(k => new SelectItem(k.timeSpan, k.label));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Openingsuur kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het benaming. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Openingsuur bijgewerkt!')
        .withMessage('Openingsuur is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Openingsuur kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
