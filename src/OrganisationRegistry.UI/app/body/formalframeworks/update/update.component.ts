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
  UpdateBodyFormalFrameworkRequest,
  BodyFormalFrameworkService
} from 'services/bodyformalframeworks';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class BodyFormalFrameworksUpdateBodyFormalFrameworkComponent implements OnInit {
  public form: FormGroup;
  public formalFramework: SearchResult;

  private readonly updateAlerts = new UpdateAlertMessages('Toepassingsgebied');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyFormalFrameworkService: BodyFormalFrameworkService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      bodyFormalFrameworkId: ['', required],
      bodyId: ['', required],
      formalFrameworkId: ['', required],
      formalFrameworkName: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodyFormalFrameworkId = res[1]['id'];

        this.bodyFormalFrameworkService
          .get(orgId, bodyFormalFrameworkId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyFormalFrameworkRequest) {
    this.form.disable();

    this.bodyFormalFrameworkService.update(value.bodyId, value)
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

  private setForm(bodyFormalFramework) {
    if (bodyFormalFramework) {
      this.form.setValue(bodyFormalFramework);
      this.formalFramework = new SearchResult(bodyFormalFramework.formalFrameworkId, bodyFormalFramework.formalFrameworkName);
    }

    this.form.enable();
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
