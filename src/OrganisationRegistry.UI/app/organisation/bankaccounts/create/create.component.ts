import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { isValidIBAN, isValidBIC } from 'ibantools';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationBankAccountService,
  CreateOrganisationBankAccountRequest
} from 'services/organisationbankaccounts';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})

export class OrganisationBankAccountsCreateOrganisationBankAccountComponent implements OnInit {
  public form: FormGroup;

  private readonly createAlerts = new CreateAlertMessages('Bankrekeningnummers');

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

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationBankAccountRequest) {
    this.form.disable();

    this.organisationBankAccountService.create(value.organisationId, value)
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
      );
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
