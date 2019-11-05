import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationCapacityFilter,
  OrganisationCapacityService,
  OrganisationCapacity
} from 'services/organisationcapacities';

@Component({
  selector: 'ww-organisation-capacity-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationCapacitiesFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationCapacityFilter>> = new EventEmitter<SearchEvent<OrganisationCapacityFilter>>();

  public organisationCapacityForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationCapacityForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationCapacityService: OrganisationCapacityService,
    private alertService: AlertService
  ) {
    this.organisationCapacityForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationCapacityForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationCapacityForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationCapacityFilter>(new OrganisationCapacityFilter(!this.organisationCapacityForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationCapacityFilter>(new OrganisationCapacityFilter(!value.showHistory)));
  }
}
