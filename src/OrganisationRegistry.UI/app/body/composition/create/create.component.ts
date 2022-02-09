import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { SelectItem } from 'shared/components/form/form-group-select';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { required } from 'core/validation';

import {
  CreateBodySeatRequest,
  BodySeatService
} from 'services/bodyseats';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodySeatsCreateBodySeatComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public seatTypes: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodySeatService: BodySeatService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodySeatId: ['', required],
      bodyId: ['', required],
      name: ['', required],
      seatTypeId: ['', required],
      paidSeat: [false],
      entitledToVote: [false],
      validFrom: [''],
      validTo: ['']
    });
  }

  ngOnInit() {
    this.seatTypes = this.route.snapshot.data['seatTypes'].map(k => new SelectItem(k.id, k.name));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateBodySeatRequest(params['id']));
    });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodySeatRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodySeatService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Post aangemaakt!')
                .withMessage('Post is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Post kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build())));
  }
}
