import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { required } from 'core/validation';

import { BodyService, BodyBalancedParticipation } from 'services/bodies';
import { RadioItem } from 'shared/components/form/form-group-radio/form-group-radio.model';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'manage.template.html',
  styleUrls: ['manage.style.css']
})
export class BodyParticipationManageComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public mepComplianceOptions: RadioItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyService: BodyService,
    private alertService: AlertService
  ) {
    this.mepComplianceOptions = [
      new RadioItem('Niet ingevuld', ''),
      new RadioItem('Mep-plichtig', 'true'),
      new RadioItem('Niet Mep-plichtig', 'false'),
    ];

    this.form = formBuilder.group({
      id: ['', required],
      obligatory: [''],
      extraRemark: ['', Validators.nullValidator],
      exceptionMeasure: ['', Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      let bodyId = params['id'];
      this.form.disable();

      this.subscriptions.push(this.bodyService.getBalancedParticipation(bodyId)
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
              .withTitle('Orgaan MEP beheer kon niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van het orgaan MEP beheer. Probeer het later opnieuw.')
              .build())));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  submit(value: BodyBalancedParticipation) {
    this.form.disable();
    this.subscriptions.push(this.bodyService.changeBalancedParticipation(value)
      .finally(() => this.form.enable())
      .subscribe(
        result => this.onSuccess(result),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Informatie orgaan MEP beheer kon niet bewaard worden!')
            .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
            .build())));
  }

  private onSuccess(result) {
    if (result) {
      this.router.navigate(['./../'], { relativeTo: this.route });

      this.alertService.setAlert(
        new AlertBuilder()
          .success()
          .withTitle('Orgaan MEP beheer bijgewerkt!')
          .withMessage('De MEP beheersinformatie van het orgaan is succesvol bijgewerkt.')
          .build());
    }
  }
}
