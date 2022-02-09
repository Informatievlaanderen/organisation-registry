import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { ContactTypeService } from 'services/contacttypes';

import {
  BodyContactService,
  UpdateBodyContactRequest
} from 'services/bodycontacts';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class BodyContactsUpdateBodyContactComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public contactTypes: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private contactTypeService: ContactTypeService,
    private bodyContactService: BodyContactService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyContactId: ['', required],
      bodyId: ['', required],
      contactTypeId: ['', required],
      contactTypeName: ['', required],
      contactValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let allContactTypesObservable = this.contactTypeService.getAllContactTypes();

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let contactId = res[1]['id'];

        this.subscriptions.push(Observable.zip(this.bodyContactService.get(orgId, contactId), allContactTypesObservable)
          .subscribe(
            item => this.setForm(item[0], item[1]),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateBodyContactRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodyContactService.update(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      ));
  }

  private setForm(bodyContact, allContactTypes) {
    if (bodyContact) {
      this.form.setValue(bodyContact);
    }

    this.contactTypes = allContactTypes.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Contact kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het contact. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Contact bijgewerkt!')
        .withMessage('Contact is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Contact kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
