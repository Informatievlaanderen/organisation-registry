import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodyFormalFrameworkFilter,
  BodyFormalFrameworkService,
  BodyFormalFramework
} from 'services/bodyformalframeworks';

@Component({
  selector: 'ww-body-formal-framework-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyFormalFrameworksFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodyFormalFrameworkFilter>> = new EventEmitter<SearchEvent<BodyFormalFrameworkFilter>>();

  public bodyFormalFrameworkForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodyFormalFrameworkForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyFormalFrameworkService: BodyFormalFrameworkService,
    private alertService: AlertService
  ) {
    this.bodyFormalFrameworkForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodyFormalFrameworkForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodyFormalFrameworkForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodyFormalFrameworkFilter>(new BodyFormalFrameworkFilter(!this.bodyFormalFrameworkForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyFormalFrameworkFilter>(new BodyFormalFrameworkFilter(!value.showHistory)));
  }
}
