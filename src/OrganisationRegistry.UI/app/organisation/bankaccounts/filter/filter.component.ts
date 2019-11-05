import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationBankAccountFilter,
  OrganisationBankAccountService,
  OrganisationBankAccount
} from 'services/organisationbankaccounts';

@Component({
  selector: 'ww-organisation-bank-account-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationBankAccountsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationBankAccountFilter>> = new EventEmitter<SearchEvent<OrganisationBankAccountFilter>>();

  public organisationBankAccountForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationBankAccountForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationBankAccountService: OrganisationBankAccountService,
    private alertService: AlertService
  ) {
    this.organisationBankAccountForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationBankAccountForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationBankAccountForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationBankAccountFilter>(new OrganisationBankAccountFilter(!this.organisationBankAccountForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationBankAccountFilter>(new OrganisationBankAccountFilter(!value.showHistory)));
  }
}
