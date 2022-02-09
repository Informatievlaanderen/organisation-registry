import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { AlertBuilder, AlertService } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { required } from 'core/validation';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { isValidIBAN, isValidBIC } from 'ibantools';

import {
  UpdateOrganisationBankAccountRequest,
  OrganisationBankAccountService
} from 'services/organisationbankaccounts';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationBankAccountsUpdateOrganisationBankAccountComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public bankAccount: SearchResult;

  private bankAccountId: string;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationBankAccountService: OrganisationBankAccountService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationBankAccountId: ['', required],
      organisationId: ['', required],
      bankAccountNumber: ['', required],
      bic: [''],
      isIban: [false, Validators.required],
      isBic: [false, Validators.required],
      validFrom: [''],
      validTo: [''],
      source: [''],
      isEditable: [false]
    });
  }

  ngOnInit() {
    this.subscriptions.push(Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let organisationBankAccountId = res[1]['id'];

        this.subscriptions.push(this.organisationBankAccountService
          .get(orgId, organisationBankAccountId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationBankAccountRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationBankAccountService.update(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleError(error)
      ));
  }

  private setForm(organisationBankAccount) {
    if (organisationBankAccount) {
      this.form.setValue(organisationBankAccount);
      this.bankAccount = new SearchResult(
              organisationBankAccount.bankAccountId,
              organisationBankAccount.bankAccountName);
    }

    this.form.enable();
  }

  onBankAccountNumberChanged(e) {
    let ibanControl = this.form.get('bankAccountNumber');
    let ibanValue : string = ibanControl.value;
    let ibanUpperCased = ibanValue.toUpperCase();

    ibanControl.setValue(ibanUpperCased);
    this.form.get('isIban').setValue(isValidIBAN(ibanUpperCased));
  }

  onBicNumberChanged(e) {
    let bicControl = this.form.get('bic');
    let bicValue : string = bicControl.value;
    let bicUpperCased = bicValue.toUpperCase();

    bicControl.setValue(bicUpperCased);
    this.form.get('isBic').setValue(isValidBIC(bicUpperCased));
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Bankrekeningnummer kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het bankrekeningnummer. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Bankrekeningnummer bijgewerkt!')
        .withMessage('Bankrekeningnummer is succesvol bijgewerkt.')
        .build());
  }
}
