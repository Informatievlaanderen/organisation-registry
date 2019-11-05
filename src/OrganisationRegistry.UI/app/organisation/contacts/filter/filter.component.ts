import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationContactFilter,
  OrganisationContactService,
  OrganisationContact
} from 'services/organisationcontacts';

@Component({
  selector: 'ww-organisation-contact-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationContactsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<OrganisationContactFilter>> = new EventEmitter<SearchEvent<OrganisationContactFilter>>();

  public organisationContactForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.organisationContactForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationContactService: OrganisationContactService,
    private alertService: AlertService
  ) {
    this.organisationContactForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.organisationContactForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationContactForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<OrganisationContactFilter>(new OrganisationContactFilter(!this.organisationContactForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationContactFilter>(new OrganisationContactFilter(!value.showHistory)));
  }
}
