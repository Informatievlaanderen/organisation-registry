import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationLabelRequest,
  OrganisationLabelService
} from 'services/organisationlabels';

import { LabelType, LabelTypeService } from 'services/labeltypes';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationLabelsCreateOrganisationLabelComponent implements OnInit {
  public form: FormGroup;
  public labelTypes: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Benaming');

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
      labelValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationLabelRequest(params['id']));
    });

    this.labelTypeService
      .getAllUserPermittedLabelTypes()
      .finally(() => this.form.enable())
      .subscribe(
        allLabelTypes => this.labelTypes = allLabelTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Benaming types konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de benaming types. Probeer het later opnieuw.')
              .build()));
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationLabelRequest) {
    this.form.disable();

    this.organisationLabelService.create(value.organisationId, value)
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
}
