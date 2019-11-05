import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { OrganisationClassificationType, OrganisationClassificationTypeService } from 'services/organisationclassificationtypes';
import { OrganisationClassification, OrganisationClassificationService } from 'services/organisationclassifications';

import {
  CreateOrganisationOrganisationClassificationRequest,
  OrganisationOrganisationClassificationService
} from 'services/organisationorganisationclassifications';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationOrganisationClassificationsCreateOrganisationOrganisationClassificationComponent implements OnInit {
  public form: FormGroup;
  public organisationClassificationTypes: SelectItem[];

  public classifications: Array<SelectItem> = [];

  private classificationType: string = '';
  private readonly createAlerts = new CreateAlertMessages('Classificatie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationClassificationTypeService: OrganisationClassificationTypeService,
    private organisationClassificationService: OrganisationClassificationService,
    private organisationOrganisationClassificationService: OrganisationOrganisationClassificationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationOrganisationClassificationId: ['', required],
      organisationId: ['', required],
      organisationClassificationTypeId: ['', required],
      organisationClassificationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationOrganisationClassificationRequest(params['id']));
    });

    this.organisationClassificationTypeService
      .getAllUserPermittedOrganisationClassificationTypes()
      .finally(() => this.enableForm())
      .subscribe(
        allOrganisationClassificationTypes => this.organisationClassificationTypes = allOrganisationClassificationTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Classificatietypes konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de classificatietypes. Probeer het later opnieuw.')
              .build()));

    this.subcribeToFormChanges();
  }

  subcribeToFormChanges() {
    const classificationTypeChanges$ = this.form.controls['organisationClassificationTypeId'].valueChanges;

    classificationTypeChanges$
      .subscribe(function (classificationType) {
        if (this.classificationType === classificationType)
          return;

        this.classificationType = classificationType;

        this.form.patchValue({ organisationClassificationId: '' });

        this.form.disable();

        if (classificationType) {

          this.organisationClassificationService
            .getAllOrganisationClassifications(classificationType)
            .finally(() => this.enableForm())
            .subscribe(
              allClassifications => this.classifications = allClassifications.map(c => new SelectItem(c.id, c.name)),
              error =>
                this.alertService.setAlert(
                  new AlertBuilder()
                    .error(error)
                    .withTitle('Classificaties konden niet geladen worden!')
                    .withMessage('Er is een fout opgetreden bij het ophalen van de classificaties. Probeer het later opnieuw.')
                    .build()));
        } else {
          this.enableForm();
        }
      }.bind(this));
  }

  enableForm() {
    this.form.enable();
    if (!this.classificationType)
      this.form.get('organisationClassificationId').disable();
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationOrganisationClassificationRequest) {
    this.form.disable();

    this.organisationOrganisationClassificationService.create(value.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Classificatie aangemaakt!')
                .withMessage('Classificatie is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Classificatie kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
