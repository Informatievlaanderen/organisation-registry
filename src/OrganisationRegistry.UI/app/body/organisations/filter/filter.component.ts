import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodyOrganisationFilter,
  BodyOrganisationService,
  BodyOrganisation
} from 'services/bodyorganisations';

@Component({
  selector: 'ww-body-organisation-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyOrganisationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodyOrganisationFilter>> = new EventEmitter<SearchEvent<BodyOrganisationFilter>>();

  public bodyOrganisationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodyOrganisationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyOrganisationService: BodyOrganisationService,
    private alertService: AlertService
  ) {
    this.bodyOrganisationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodyOrganisationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodyOrganisationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodyOrganisationFilter>(new BodyOrganisationFilter(!this.bodyOrganisationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyOrganisationFilter>(new BodyOrganisationFilter(!value.showHistory)));
  }
}
