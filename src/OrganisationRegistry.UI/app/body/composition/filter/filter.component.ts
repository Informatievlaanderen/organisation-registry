import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodySeatFilter,
  BodySeatService,
  BodySeat
} from 'services/bodyseats';

@Component({
  selector: 'ww-body-seat-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodySeatFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodySeatFilter>> = new EventEmitter<SearchEvent<BodySeatFilter>>();

  public bodySeatForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodySeatForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodySeatService: BodySeatService,
    private alertService: AlertService
  ) {
    this.bodySeatForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodySeatForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodySeatForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodySeatFilter>(new BodySeatFilter(!this.bodySeatForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodySeatFilter>(new BodySeatFilter(!value.showHistory)));
  }
}
