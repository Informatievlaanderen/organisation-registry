import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { PurposeFilter } from 'services/purposes';

@Component({
  selector: 'ww-purpose-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class PurposeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<PurposeFilter>> = new EventEmitter<SearchEvent<PurposeFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name')
      ));
  }

  @Input('isBusy')
  set name(isBusy: boolean) {
    if (isBusy) {
      this.form.disable();
    } else {
      this.form.enable();
    }
  }

  ngOnInit() {
    this.form.setValue(new PurposeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<PurposeFilter>(this.form.value));
  }

  filterForm(value: PurposeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<PurposeFilter>(value));
  }
}
