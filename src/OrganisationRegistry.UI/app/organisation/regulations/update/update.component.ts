import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { RegulationType, RegulationTypeService } from 'services/regulationtypes';

import {
  OrganisationRegulationService,
  UpdateOrganisationRegulationRequest
} from 'services/organisationregulations';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationRegulationsUpdateOrganisationRegulationComponent implements OnInit {
  public form: FormGroup;
  public regulationTypes: SelectItem[];

  private readonly updateAlerts = new UpdateAlertMessages('Regelgeving');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private regulationTypeService: RegulationTypeService,
    private organisationRegulationService: OrganisationRegulationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationRegulationId: ['', required],
      organisationId: ['', required],
      regulationTypeId: [''],
      regulationTypeName: [''],
      link: [''],
      date: [''],
      description: [''],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let allRegulationTypesObservable = this.regulationTypeService.getAllRegulationTypes();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let regulationId = res[1]['id'];

        Observable.zip(this.organisationRegulationService.get(orgId, regulationId), allRegulationTypesObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationRegulationRequest) {
    this.form.disable();

    this.organisationRegulationService.update(value.organisationId, value)
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

  private setForm(organisationRegulation, allRegulationTypes) {
    if (organisationRegulation) {
      this.form.setValue(organisationRegulation);
    }

    this.regulationTypes = allRegulationTypes.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Regelgeving kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de regelgeving. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Regelgeving bijgewerkt!')
        .withMessage('Regelgeving is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Regelgeving kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
