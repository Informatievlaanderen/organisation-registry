import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationContactRequest,
  OrganisationContactService
} from 'services/organisationcontacts';

import { ContactTypeService } from 'services/contacttypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationContactsCreateOrganisationContactComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public contactTypes: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private contactTypeService: ContactTypeService,
    private organisationContactService: OrganisationContactService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationContactId: ['', required],
      organisationId: ['', required],
      contactTypeId: ['', required],
      contactValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationContactRequest(params['id']));
    });

    this.subscriptions.push(this.contactTypeService
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
              .build())));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationContactRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationContactService.create(value.organisationId, value)
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
              .build())));
  }
}
