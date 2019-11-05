import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationRelationFilter,
  OrganisationRelationService,
  OrganisationRelation
} from 'services/organisationrelations';

@Component({
  selector: 'ww-organisation-relation-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationRelationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationRelationFilter>> = new EventEmitter<SearchEvent<OrganisationRelationFilter>>();

  public organisationRelationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationRelationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationRelationService: OrganisationRelationService,
    private alertService: AlertService
  ) {
    this.organisationRelationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationRelationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationRelationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationRelationFilter>(new OrganisationRelationFilter(!this.organisationRelationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationRelationFilter>(new OrganisationRelationFilter(!value.showHistory)));
  }
}
