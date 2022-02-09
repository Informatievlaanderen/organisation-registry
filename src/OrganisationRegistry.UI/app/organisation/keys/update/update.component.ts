import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import { KeyTypeService } from 'services/keytypes';

import {
  UpdateOrganisationKeyRequest,
  OrganisationKeyService
} from 'services/organisationkeys';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationKeysUpdateOrganisationKeyComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public keys: SelectItem[];
  private keyId: string;

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
      keyTypeName: ['', required],
      keyValue: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let allKeysObservable = this.keyService.getAllKeyTypes();

    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let keyId = res[1]['id'];

        this.subscriptions.push(Observable.zip(this.organisationKeyService.get(orgId, keyId), allKeysObservable)
          .subscribe(item => this.setForm(item[0], item[1]),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationKeyRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationKeyService.update(value.organisationId, value)
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

  private setForm(organisationKey, allKeys) {
    if (organisationKey) {
      this.form.setValue(organisationKey);
    }

    this.keys = allKeys.map(k => new SelectItem(k.id, k.name));
    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Sleutel kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de sleutel. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Sleutel bijgewerkt!')
        .withMessage('Sleutel is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Sleutel kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
