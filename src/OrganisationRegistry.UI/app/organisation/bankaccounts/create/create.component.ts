import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { isValidIBAN, isValidBIC } from 'ibantools';

import {
  OrganisationBankAccountService,
  CreateOrganisationBankAccountRequest
} from 'services/organisationbankaccounts';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})

export class OrganisationBankAccountsCreateOrganisationBankAccountComponent implements OnInit, OnDestroy {
  public form: FormGroup;

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
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationBankAccountRequest(params['id']));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationBankAccountRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationBankAccountService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
        if (result) {
          this.router.navigate(['./..'], { relativeTo: this.route });

          this.alertService.setAlert(
            new AlertBuilder()
              .success()
              .withTitle('Bankrekeningnummer gekoppeld!')
              .withMessage('Bankrekeningnummer is succesvol gekoppeld.')
              .build());
        }
      },
      error =>
        this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Bankrekeningnummer kon niet gekoppeld worden!')
            .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
            .build())
      ));
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
}
