import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationOrganisationClassificationFilter,
  OrganisationOrganisationClassificationService,
  OrganisationOrganisationClassification
} from 'services/organisationorganisationclassifications';

@Component({
  selector: 'ww-organisation-organisationclassification-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationOrganisationClassificationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationOrganisationClassificationFilter>> = new EventEmitter<SearchEvent<OrganisationOrganisationClassificationFilter>>();

  public organisationOrganisationClassificationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationOrganisationClassificationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationOrganisationClassificationService: OrganisationOrganisationClassificationService,
    private alertService: AlertService
  ) {
    this.organisationOrganisationClassificationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationOrganisationClassificationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationOrganisationClassificationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationOrganisationClassificationFilter>(new OrganisationOrganisationClassificationFilter(!this.organisationOrganisationClassificationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationOrganisationClassificationFilter>(new OrganisationOrganisationClassificationFilter(!value.showHistory)));
  }
}
