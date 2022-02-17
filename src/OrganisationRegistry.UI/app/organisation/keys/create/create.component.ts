import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { KeyTypeService } from 'services/keytypes';

import {
  CreateOrganisationKeyRequest,
  OrganisationKeyService
} from 'services/organisationkeys';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationKeysCreateOrganisationKeyComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public keys: SelectItem[];

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private keyService: KeyTypeService,
    private organisationKeyService: OrganisationKeyService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationKeyId: ['', required],
      organisationId: ['', required],
      keyTypeId: ['', required],
      keyValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();
    this.route.parent.parent.params.forEach((params: Params) => {
      let organisationId = params['id'];
      this.form.setValue(new CreateOrganisationKeyRequest(organisationId));

      this.subscriptions.push(this.keyService
        .getAllKeyTypes(organisationId)
        .finally(() => this.form.enable())
        .subscribe(
          allKeys => this.keys = allKeys.map(k => new SelectItem(k.id, k.name)),
          error =>
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle('Informatiesystemen konden niet geladen worden!')
                .withMessage('Er is een fout opgetreden bij het ophalen van de informatiesystemen. Probeer het later opnieuw.')
                .build())));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationKeyRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationKeyService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Sleutel aangemaakt!')
                .withMessage('Sleutel is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Sleutel kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build())));
  }
}
