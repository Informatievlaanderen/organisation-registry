import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodyContactFilter,
  BodyContactService,
  BodyContact
} from 'services/bodycontacts';

@Component({
  selector: 'ww-body-contact-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyContactsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodyContactFilter>> = new EventEmitter<SearchEvent<BodyContactFilter>>();

  public bodyContactForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodyContactForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyContactService: BodyContactService,
    private alertService: AlertService
  ) {
    this.bodyContactForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodyContactForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodyContactForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodyContactFilter>(new BodyContactFilter(!this.bodyContactForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyContactFilter>(new BodyContactFilter(!value.showHistory)));
  }
}
