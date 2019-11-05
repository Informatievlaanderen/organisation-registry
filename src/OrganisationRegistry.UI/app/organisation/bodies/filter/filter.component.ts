import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationBodyFilter,
  OrganisationBodyService,
  OrganisationBody
} from 'services/organisationbodies';

@Component({
  selector: 'ww-organisation-body-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationBodiesFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationBodyFilter>> = new EventEmitter<SearchEvent<OrganisationBodyFilter>>();

  public organisationBodyForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationBodyForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationBodyService: OrganisationBodyService,
    private alertService: AlertService
  ) {
    this.organisationBodyForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationBodyForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationBodyForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationBodyFilter>(new OrganisationBodyFilter(!this.organisationBodyForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationBodyFilter>(new OrganisationBodyFilter(!value.showHistory)));
  }
}
