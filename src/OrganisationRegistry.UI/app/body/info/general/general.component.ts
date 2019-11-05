import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { BodyService, Body, BodyInfo } from 'services/bodies';

@Component({
  templateUrl: 'general.template.html',
  styleUrls: ['general.style.css']
})
export class BodyInfoGeneralComponent implements OnInit {
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
      name: ['', required],
      shortName: ['', Validators.nullValidator],
      description: ['', Validators.nullValidator],
      bodyNumber: ['', Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      let bodyId = params['id'];
      this.form.disable();

      this.bodyService.getInfo(bodyId)
        .finally(() => this.enableForm())
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

  submit(value: BodyInfo) {
    this.form.disable();
    this.bodyService.changeInfo(value)
      .finally(() => this.enableForm())
      .subscribe(
        result => this.onSuccess(result),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Informatie orgaan kon niet bewaard worden!')
            .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
            .build()));
  }

  private enableForm() {
    this.form.enable();
    this.form.get('bodyNumber').disable();
  }

  private onSuccess(result) {
    if (result) {
      this.router.navigate(['./../'], { relativeTo: this.route });

      this.alertService.setAlert(
        new AlertBuilder()
          .success()
          .withTitle('Informatie orgaan bijgewerkt!')
          .withMessage('De algemene informatie van het orgaan is succesvol bijgewerkt.')
          .build());
    }
  }
}
