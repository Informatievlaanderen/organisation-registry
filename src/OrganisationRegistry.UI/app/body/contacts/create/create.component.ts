import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateBodyContactRequest,
  BodyContactService
} from 'services/bodycontacts';

import { ContactType, ContactTypeService } from 'services/contacttypes';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyContactsCreateBodyContactComponent implements OnInit {
  public form: FormGroup;
  public contactTypes: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Contact');

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
      contactValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();

    this.route.parent.parent.params.forEach((params: Params) => {

      this.form.setValue(new CreateBodyContactRequest(params['id']));
    });

    this.contactTypeService
      .getAllContactTypes()
      .finally(() => this.form.enable())
      .subscribe(
        allContactTypes => this.contactTypes = allContactTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Contact types konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de contact types. Probeer het later opnieuw.')
              .build()));
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyContactRequest) {
    this.form.disable();

    this.bodyContactService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Contact aangemaakt!')
                .withMessage('Contact is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Contact kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
