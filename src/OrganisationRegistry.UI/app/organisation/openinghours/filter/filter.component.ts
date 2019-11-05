import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationOpeningHourFilter,
  OrganisationOpeningHourService,
  OrganisationOpeningHour
} from 'services/organisationopeninghours';

@Component({
  selector: 'ww-organisation-openinghour-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationOpeningHoursFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationOpeningHourFilter>> = new EventEmitter<SearchEvent<OrganisationOpeningHourFilter>>();

  public organisationOpeningHourForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationOpeningHourForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationOpeningHourService: OrganisationOpeningHourService,
    private alertService: AlertService
  ) {
    this.organisationOpeningHourForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationOpeningHourForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationOpeningHourForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationOpeningHourFilter>(new OrganisationOpeningHourFilter(!this.organisationOpeningHourForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationOpeningHourFilter>(new OrganisationOpeningHourFilter(!value.showHistory)));
  }
}
