import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationRegulationFilter,
  OrganisationRegulationService,
  OrganisationRegulation
} from 'services/organisationregulations';

@Component({
  selector: 'ww-organisation-regulation-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationRegulationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationRegulationFilter>> = new EventEmitter<SearchEvent<OrganisationRegulationFilter>>();

  public organisationRegulationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationRegulationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationRegulationService: OrganisationRegulationService,
    private alertService: AlertService
  ) {
    this.organisationRegulationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationRegulationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationRegulationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationRegulationFilter>(new OrganisationRegulationFilter(!this.organisationRegulationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationRegulationFilter>(new OrganisationRegulationFilter(!value.showHistory)));
  }
}
