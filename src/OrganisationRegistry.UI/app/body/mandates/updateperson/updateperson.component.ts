import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, FormControl} from '@angular/forms';
import {ActivatedRoute, Router, Params} from '@angular/router';

import {Observable} from 'rxjs/Observable';

import {AlertService, AlertBuilder} from 'core/alert';
import {required} from 'core/validation';

import {SelectItem} from 'shared/components/form/form-group-select';
import {SearchResult} from 'shared/components/form/form-group-autocomplete';
import {ContactTypeListItem} from 'services/contacttypes';

import {
  UpdateBodyMandateRequest,
  BodyMandateService
} from 'services/bodymandates';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'updateperson.template.html',
  styleUrls: ['updateperson.style.css']
})
export class BodyMandatesUpdatePersonComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public isBusy: boolean;
  public bodySeats: SelectItem[];
  public person: SearchResult;
  public contactTypes: ContactTypeListItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

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
      delegatedId: [''],

      bodySeatName: [''],
      bodySeatNumber: [''],
      delegatedName: [''],
      delegatorName: [''],
      assignedToId: [],
      assignedToName: []
    });
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.bodySeats = this
        .route
        .snapshot.data['bodySeats']
        .map(k => new SelectItem(k.bodySeatId, `${k.name} - ${k.seatTypeName} (#${k.bodySeatNumber})`));
    });

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let bodyMandateId = res[1]['id'];

        this.subscriptions.push(this.bodyMandateService
          .get(orgId, bodyMandateId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  toFormGroup(contactTypes: ContactTypeListItem[]) {
    let group: any = {};

    contactTypes.forEach(contactType => {
      group[contactType.id] = new FormControl('');
    });

    return new FormGroup(group);
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyMandateRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodyMandateService.update(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], {relativeTo: this.route});
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      ));
  }

  private setForm(bodyMandate) {
    if (bodyMandate) {
      if (!bodyMandate.contacts)
        bodyMandate.contacts = {};

      this.contactTypes.forEach(contactType => {
        if (!bodyMandate.contacts[contactType.id])
          bodyMandate.contacts[contactType.id] = '';
      });

      this.form.setValue(bodyMandate);
      this.person = new SearchResult(bodyMandate.delegatorId, bodyMandate.delegatorName);
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
