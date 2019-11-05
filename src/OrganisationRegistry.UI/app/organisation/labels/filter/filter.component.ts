import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationLabelFilter,
  OrganisationLabelService,
  OrganisationLabel
} from 'services/organisationlabels';

@Component({
  selector: 'ww-organisation-label-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationLabelsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationLabelFilter>> = new EventEmitter<SearchEvent<OrganisationLabelFilter>>();

  public organisationLabelForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationLabelForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationLabelService: OrganisationLabelService,
    private alertService: AlertService
  ) {
    this.organisationLabelForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationLabelForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationLabelForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationLabelFilter>(new OrganisationLabelFilter(!this.organisationLabelForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationLabelFilter>(new OrganisationLabelFilter(!value.showHistory)));
  }
}
