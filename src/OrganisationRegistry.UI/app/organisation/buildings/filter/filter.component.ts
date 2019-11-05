import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationBuildingFilter,
  OrganisationBuildingService,
  OrganisationBuilding
} from 'services/organisationbuildings';

@Component({
  selector: 'ww-organisation-building-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationBuildingsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationBuildingFilter>> = new EventEmitter<SearchEvent<OrganisationBuildingFilter>>();

  public organisationBuildingForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationBuildingForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationBuildingService: OrganisationBuildingService,
    private alertService: AlertService
  ) {
    this.organisationBuildingForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationBuildingForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationBuildingForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationBuildingFilter>(new OrganisationBuildingFilter(!this.organisationBuildingForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationBuildingFilter>(new OrganisationBuildingFilter(!value.showHistory)));
  }
}
