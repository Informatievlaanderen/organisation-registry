import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodyMandateFilter,
  BodyMandateService,
  BodyMandate
} from 'services/bodymandates';

@Component({
  selector: 'ww-body-mandate-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyMandatesFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodyMandateFilter>> = new EventEmitter<SearchEvent<BodyMandateFilter>>();

  public bodyMandateForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodyMandateForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyMandateService: BodyMandateService,
    private alertService: AlertService
  ) {
    this.bodyMandateForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodyMandateForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodyMandateForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodyMandateFilter>(new BodyMandateFilter(!this.bodyMandateForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyMandateFilter>(new BodyMandateFilter(!value.showHistory)));
  }
}
