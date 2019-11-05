import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  BodyFilter,
  BodyService,
  Body
} from 'services/bodies';

@Component({
  selector: 'ww-body-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<BodyFilter>> = new EventEmitter<SearchEvent<BodyFilter>>();

  public bodyForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;
  private isBusyExternal: boolean = true;

  get isFormValid() {
    return this.bodyForm.enabled && this.bodyForm.valid;
  }

  updateFormEnabled(isBusyInternal, isBusyExternal) {
    this.isBusyInternal = isBusyInternal;
    this.isBusyExternal = isBusyExternal;

    if (this.isBusyInternal || this.isBusyExternal) {
      this.bodyForm.disable();
    } else {
      this.bodyForm.enable();
    }
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyService: BodyService,
    private alertService: AlertService
  ) {
    this.bodyForm = formBuilder.group({
      name: ['', Validators.nullValidator],
      organisation: ['', Validators.nullValidator],
      activeOnly: [true, Validators.nullValidator]
    });
  }

  @Input('isBusy')
  set name(isBusy: boolean) {
    this.updateFormEnabled(this.isBusyInternal, isBusy);
  }

  ngOnInit() {
    this.bodyForm.setValue(new BodyFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.bodyForm.reset({
      name: '',
      organisation: '',
      activeOnly: true
    });

    this.filter.emit(new SearchEvent<BodyFilter>(this.bodyForm.value));
  }

  filterForm(value: BodyFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyFilter>(value));
  }
}
