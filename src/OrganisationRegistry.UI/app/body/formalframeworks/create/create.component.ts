import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateBodyFormalFrameworkRequest,
  BodyFormalFrameworkService
} from 'services/bodyformalframeworks';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyFormalFrameworksCreateBodyFormalFrameworkComponent implements OnInit {
  public form: FormGroup;

  private readonly createAlerts = new CreateAlertMessages('Toepassingsgebied');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyFormalFrameworkService: BodyFormalFrameworkService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyFormalFrameworkId: ['', required],
      bodyId: ['', required],
      formalFrameworkId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateBodyFormalFrameworkRequest(params['id']));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyFormalFrameworkRequest) {
    this.form.disable();

    this.bodyFormalFrameworkService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Toepassingsgebied gekoppeld!')
                .withMessage('Toepassingsgebied is succesvol gekoppeld.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Toepassingsgebied kon niet gekoppeld worden!')
              .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
