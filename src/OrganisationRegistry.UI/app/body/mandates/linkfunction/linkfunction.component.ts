import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateBodyMandateRequest,
  BodyMandateService,
  BodyMandateType
} from 'services/bodymandates';

@Component({
  templateUrl: 'linkfunction.template.html',
  styleUrls: ['linkfunction.style.css']
})
export class BodyMandatesLinkFunctionComponent implements OnInit {
  public form: FormGroup;
  public bodySeats: SelectItem[];
  public functionTypes: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Mandaat');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyMandateService: BodyMandateService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyMandateId: ['', required],
      bodyId: ['', required],
      bodySeatId: ['', required],
      validFrom: [''],
      validTo: [''],

      bodyMandateType: [''],
      delegatorId: ['', required],
      delegatedId: ['', required],

      contacts: []
    });
  }

  ngOnInit() {
    this.functionTypes = this.route.snapshot.data['functionTypes'].map(k => new SelectItem(k.id, k.name));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.bodySeats = this
        .route
        .snapshot.data['bodySeats']
        .map(k => new SelectItem(k.bodySeatId, `${k.name} - ${k.seatTypeName} (#${k.bodySeatNumber})`));

      this.form.setValue(new CreateBodyMandateRequest(params['id'], BodyMandateType.Function));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyMandateRequest) {
    this.form.disable();

    this.bodyMandateService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Mandaat toegewezen!')
                .withMessage('Mandaat is succesvol toegewezen.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Mandaat kon niet toegewezen worden!')
              .withMessage('Er is een fout opgetreden bij het toewijzen van het mandaat. Probeer het later opnieuw.')
              .build()));
  }
}
