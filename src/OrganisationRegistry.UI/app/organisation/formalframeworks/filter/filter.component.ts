import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationFormalFrameworkFilter,
  OrganisationFormalFrameworkService,
  OrganisationFormalFramework
} from 'services/organisationformalframeworks';

@Component({
  selector: 'ww-organisation-formal-framework-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationFormalFrameworksFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationFormalFrameworkFilter>> = new EventEmitter<SearchEvent<OrganisationFormalFrameworkFilter>>();

  public organisationFormalFrameworkForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationFormalFrameworkForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationFormalFrameworkService: OrganisationFormalFrameworkService,
    private alertService: AlertService
  ) {
    this.organisationFormalFrameworkForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationFormalFrameworkForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationFormalFrameworkForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationFormalFrameworkFilter>(new OrganisationFormalFrameworkFilter(!this.organisationFormalFrameworkForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationFormalFrameworkFilter>(new OrganisationFormalFrameworkFilter(!value.showHistory)));
  }
}
