import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { BodyClassificationTypeService } from 'services/bodyclassificationtypes';
import { BodyClassificationService } from 'services/bodyclassifications';

import {
  CreateBodyBodyClassificationRequest,
  BodyBodyClassificationService
} from 'services/bodybodyclassifications';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyBodyClassificationsCreateBodyBodyClassificationComponent implements OnInit {
  public form: FormGroup;
  public bodyClassificationTypes: SelectItem[];
  public classifications: Array<SelectItem> = [];
  private classificationType: string = '';
  private readonly createAlerts = new CreateAlertMessages('Classificatie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyClassificationTypeService: BodyClassificationTypeService,
    private bodyClassificationService: BodyClassificationService,
    private bodyBodyClassificationService: BodyBodyClassificationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyBodyClassificationId: ['', required],
      bodyId: ['', required],
      bodyClassificationTypeId: ['', required],
      bodyClassificationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateBodyBodyClassificationRequest(params['id']));
    });

    this.bodyClassificationTypeService
      .getAllBodyClassificationTypes()
      .finally(() => this.enableForm())
      .subscribe(
        allBodyClassificationTypes => this.bodyClassificationTypes = allBodyClassificationTypes.map(k => new SelectItem(k.id, k.name)),
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
    const classificationTypeChanges$ = this.form.controls['bodyClassificationTypeId'].valueChanges;

    classificationTypeChanges$
      .subscribe(function (classificationType) {
        if (this.classificationType === classificationType)
          return;

        this.classificationType = classificationType;

        this.form.patchValue({ bodyClassificationId: '' });

        this.form.disable();

        if (classificationType) {

          this.bodyClassificationService
            .getAllBodyClassifications(classificationType)
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
      this.form.get('bodyClassificationId').disable();
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyBodyClassificationRequest) {
    this.form.disable();

    this.bodyBodyClassificationService.create(value.bodyId, value)
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
