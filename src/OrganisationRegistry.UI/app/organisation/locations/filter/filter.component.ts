import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationLocationFilter,
  OrganisationLocationService,
  OrganisationLocation
} from 'services/organisationlocations';

@Component({
  selector: 'ww-organisation-location-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationLocationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationLocationFilter>> = new EventEmitter<SearchEvent<OrganisationLocationFilter>>();

  public organisationLocationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationLocationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationLocationService: OrganisationLocationService,
    private alertService: AlertService
  ) {
    this.organisationLocationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationLocationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationLocationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationLocationFilter>(new OrganisationLocationFilter(!this.organisationLocationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationLocationFilter>(new OrganisationLocationFilter(!value.showHistory)));
  }
}
