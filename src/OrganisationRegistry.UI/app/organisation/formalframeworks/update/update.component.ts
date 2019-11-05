import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import {
  UpdateOrganisationFormalFrameworkRequest,
  OrganisationFormalFrameworkService
} from 'services/organisationformalframeworks';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationFormalFrameworksUpdateOrganisationFormalFrameworkComponent implements OnInit {
  public form: FormGroup;
  public formalFramework: SearchResult;
  public parentOrganisation: SearchResult;

  private readonly updateAlerts = new UpdateAlertMessages('Toepassingsgebied');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationFormalFrameworkService: OrganisationFormalFrameworkService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      organisationFormalFrameworkId: ['', required],
      organisationId: ['', required],
      formalFrameworkId: ['', required],
      formalFrameworkName: ['', required],
      parentOrganisationId: ['', required],
      parentOrganisationName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let organisationFormalFrameworkId = res[1]['id'];

        this.organisationFormalFrameworkService
          .get(orgId, organisationFormalFrameworkId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update() {
    this.form.disable();
    let value = this.form.value;

    this.organisationFormalFrameworkService.update(value.organisationId, value)
      .finally(() => this.enableForm())
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

  private setForm(organisationFormalFramework) {
    if (organisationFormalFramework) {
      this.form.setValue(organisationFormalFramework);
      this.formalFramework = new SearchResult(organisationFormalFramework.formalFrameworkId, organisationFormalFramework.formalFrameworkName);
      this.parentOrganisation = new SearchResult(organisationFormalFramework.parentOrganisationId, organisationFormalFramework.parentOrganisationName);
    }

    this.enableForm();
  }

  private enableForm() {
    this.form.enable();
    this.form.get('formalFrameworkId').disable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Toepassingsgebied kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het toepassingsgebied. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Toepassingsgebied bijgewerkt!')
        .withMessage('Toepassingsgebied is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Toepassingsgebied kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
