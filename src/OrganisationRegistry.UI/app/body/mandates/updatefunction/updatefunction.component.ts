import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import {
  UpdateBodyMandateRequest,
  BodyMandateService,
  BodyMandateType
} from 'services/bodymandates';

@Component({
  templateUrl: 'updatefunction.template.html',
  styleUrls: ['updatefunction.style.css']
})
export class BodyMandatesUpdateFunctionComponent implements OnInit {
  public form: FormGroup;
  public bodySeats: SelectItem[];
  public functionTypes: SelectItem[];
  public organisation: SearchResult;

  private readonly updateAlerts = new UpdateAlertMessages('Mandaat');

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

      bodySeatName: [''],
      bodySeatNumber: [''],
      delegatedName: [''],
      delegatorName: [''],

      contacts: [],
      assignedToId: [],
      assignedToName: []
    });
  }

  ngOnInit() {
    this.functionTypes = this.route.snapshot.data['functionTypes'].map(k => new SelectItem(k.id, k.name));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.bodySeats = this
        .route
        .snapshot.data['bodySeats']
        .map(k => new SelectItem(k.bodySeatId, `${k.name} - ${k.seatTypeName} (#${k.bodySeatNumber})`));
    });

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodyMandateId = res[1]['id'];

        this.bodyMandateService
          .get(orgId, bodyMandateId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyMandateRequest) {
    this.form.disable();

    this.bodyMandateService.update(value.bodyId, value)
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

  private setForm(bodyMandate) {
    if (bodyMandate) {
      this.form.setValue(bodyMandate);
      this.organisation = new SearchResult(bodyMandate.delegatorId, bodyMandate.delegatorName);
    }

    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Mandaat kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het mandaat. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Mandaat bijgewerkt!')
        .withMessage('Mandaat is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Mandaat kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
