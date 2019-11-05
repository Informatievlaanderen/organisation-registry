import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { SeatTypeFilter } from 'services/seattypes';

@Component({
  selector: 'ww-seat-type-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class SeatTypeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<SeatTypeFilter>> = new EventEmitter<SearchEvent<SeatTypeFilter>>();

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
    this.form.setValue(new SeatTypeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<SeatTypeFilter>(this.form.value));
  }

  filterForm(value: SeatTypeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<SeatTypeFilter>(value));
  }
}
