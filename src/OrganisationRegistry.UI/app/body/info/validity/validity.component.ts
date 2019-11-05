import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { BodyService, Body, BodyValidity } from 'services/bodies';

@Component({
  templateUrl: 'validity.template.html',
  styleUrls: ['validity.style.css']
})
export class BodyInfoValidityComponent implements OnInit {
  public form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyService: BodyService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      formalValidFrom: [''],
      formalValidTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      let bodyId = params['id'];
      this.form.disable();

      this.bodyService.getValidity(bodyId)
        .finally(() => this.form.enable())
        .subscribe(
          item => {
            if (item) {
              this.form.setValue(item);
            }
          },
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Orgaan kon niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van het orgaan. Probeer het later opnieuw.')
              .build()));
    });
  }

  submit(value: BodyValidity) {
    this.form.disable();
    this.bodyService.changeValidity(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => this.onSuccess(result),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Duurtijd orgaan kon niet bewaard worden!')
            .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
            .build()));
  }

  private onSuccess(result) {
    if (result) {
      this.router.navigate(['./../'], { relativeTo: this.route });

      this.alertService.setAlert(
        new AlertBuilder()
          .success()
          .withTitle('Duurtijd orgaan bijgewerkt!')
          .withMessage('De duurtijd van het orgaan is succesvol bijgewerkt.')
          .build());
    }
  }
}
