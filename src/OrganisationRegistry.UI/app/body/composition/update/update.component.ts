import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { SelectItem } from 'shared/components/form/form-group-select';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { required } from 'core/validation';

import {
  UpdateBodySeatRequest,
  BodySeatService
} from 'services/bodyseats';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class BodySeatsUpdateBodySeatComponent implements OnInit {
  public form: FormGroup;
  public seatTypes: SelectItem[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodySeatService: BodySeatService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      bodySeatId: ['', required],
      bodyId: ['', required],
      name: ['', required],
      seatTypeId: ['', required],
      seatTypeName: ['', required],
      paidSeat: [false],
      entitledToVote: [false],
      validFrom: [''],
      validTo: [''],
      bodySeatNumber: ['']
    });
  }

  ngOnInit() {
    this.seatTypes = this.route.snapshot.data['seatTypes'].map(k => new SelectItem(k.id, k.name));

    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodySeatId = res[1]['id'];

        this.bodySeatService
          .get(orgId, bodySeatId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodySeatRequest) {
    this.form.disable();

    this.bodySeatService.update(value.bodyId, value)
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

  private setForm(bodySeat) {
    if (bodySeat) {
      this.form.setValue(bodySeat);
    }

    this.enableForm();
  }

  private enableForm() {
    this.form.enable();
    this.form.get('bodySeatNumber').disable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Post kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de post. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Post bijgewerkt!')
        .withMessage('Post is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Post kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
