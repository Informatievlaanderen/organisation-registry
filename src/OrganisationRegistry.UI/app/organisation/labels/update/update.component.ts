import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder } from 'core/alert';
import { required } from 'core/validation';

import { LabelTypeService } from 'services/labeltypes';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationLabelService,
  UpdateOrganisationLabelRequest
} from 'services/organisationlabels';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationLabelsUpdateOrganisationLabelComponent implements OnInit {
  public form: FormGroup;
  public labelTypes: SelectItem[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private labelTypeService: LabelTypeService,
    private organisationLabelService: OrganisationLabelService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationLabelId: ['', required],
      organisationId: ['', required],
      labelTypeId: ['', required],
      labelTypeName: ['', required],
      labelValue: ['', required],
      validFrom: [''],
      validTo: [''],
      source: [''],
      isEditable: [false]
    });
  }

  ngOnInit() {
    let allLabelTypesObservable = this.labelTypeService.getAllUserPermittedLabelTypes();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let labelId = res[1]['id'];

        Observable.zip(this.organisationLabelService.get(orgId, labelId), allLabelTypesObservable)
          .subscribe(item => this.setForm(item[0], item[1]),
          error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationLabelRequest) {
    this.form.disable();

    this.organisationLabelService.update(value.organisationId, value)
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

  private setForm(organisationLabel, allLabelTypes) {
    if (organisationLabel) {
      this.form.setValue(organisationLabel);
    }

    this.labelTypes = allLabelTypes.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Benaming kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het benaming. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Benaming bijgewerkt!')
        .withMessage('Benaming is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Benaming kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
