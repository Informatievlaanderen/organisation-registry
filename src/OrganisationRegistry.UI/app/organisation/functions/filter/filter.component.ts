import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationFunctionFilter,
  OrganisationFunctionService,
  OrganisationFunction
} from 'services/organisationfunctions';

@Component({
  selector: 'ww-organisation-function-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationFunctionsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationFunctionFilter>> = new EventEmitter<SearchEvent<OrganisationFunctionFilter>>();

  public organisationFunctionForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationFunctionForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationFunctionService: OrganisationFunctionService,
    private alertService: AlertService
  ) {
    this.organisationFunctionForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationFunctionForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationFunctionForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationFunctionFilter>(new OrganisationFunctionFilter(!this.organisationFunctionForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationFunctionFilter>(new OrganisationFunctionFilter(!value.showHistory)));
  }
}
