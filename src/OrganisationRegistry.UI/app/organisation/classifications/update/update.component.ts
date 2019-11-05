import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { OrganisationClassificationType, OrganisationClassificationTypeService } from 'services/organisationclassificationtypes';
import { OrganisationClassification, OrganisationClassificationService } from 'services/organisationclassifications';

import {
  UpdateOrganisationOrganisationClassificationRequest,
  OrganisationOrganisationClassificationService
} from 'services/organisationorganisationclassifications';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationOrganisationClassificationsUpdateOrganisationOrganisationClassificationComponent implements OnInit {
  public form: FormGroup;
  public organisationClassificationTypes: SelectItem[];
  public classifications: Array<SelectItem> = [];

  private classificationType: string = '';
  private readonly updateAlerts = new UpdateAlertMessages('Classificatie');
  private organisationClassificationId: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationClassificationTypeService: OrganisationClassificationTypeService,
    private organisationClassificationService: OrganisationClassificationService,
    private organisationOrganisationClassificationService: OrganisationOrganisationClassificationService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationOrganisationClassificationId: ['', required],
      organisationId: ['', required],
      organisationClassificationTypeId: ['', required],
      organisationClassificationTypeName: ['', required],
      organisationClassificationId: ['', required],
      organisationClassificationName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();
    let allOrganisationClassificationTypesObservable = this.organisationClassificationTypeService.getAllUserPermittedOrganisationClassificationTypes();
    this.subscribeToFormChanges();

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .finally(() => this.enableForm())
      .subscribe(res => {
        let orgId = res[0]['id'];
        let organisationClassificationId = res[1]['id'];

        Observable.zip(this.organisationOrganisationClassificationService.get(orgId, organisationClassificationId), allOrganisationClassificationTypesObservable)
          .subscribe(item => this.setForm(item[0], item[1]),
          error => this.handleError(error));
      });
  }

  subscribeToFormChanges() {
    const classificationTypeChanges$ = this.form.controls['organisationClassificationTypeId'].valueChanges;

    classificationTypeChanges$
      .subscribe(function (classificationType) {
        if (this.classificationType === classificationType)
          return;

        this.classificationType = classificationType;

        this.form.patchValue({ organisationClassificationId: '' });

        this.form.disable();

        if (classificationType) {
          this.form.controls['organisationClassificationId'].disable();

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

  update(value: UpdateOrganisationOrganisationClassificationRequest) {
    this.form.disable();

    this.organisationOrganisationClassificationService.update(value.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error));
  }

  private setForm(organisationOrganisationClassification, allOrganisationClassificationTypes) {
    if (organisationOrganisationClassification) {
      this.form.setValue(organisationOrganisationClassification);
    }

    this.organisationClassificationTypes = allOrganisationClassificationTypes.map(k => new SelectItem(k.id, k.name));
    this.enableForm();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Classificatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de classificatie. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Classificatie bijgewerkt!')
        .withMessage('Classificatie is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Classificatie kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
