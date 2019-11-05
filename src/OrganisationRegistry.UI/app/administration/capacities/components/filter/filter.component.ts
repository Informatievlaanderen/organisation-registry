import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { CapacityFilter } from 'services/capacities';

@Component({
  selector: 'ww-capacity-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class CapacityFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<CapacityFilter>> = new EventEmitter<SearchEvent<CapacityFilter>>();

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
    this.form.setValue(new CapacityFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<CapacityFilter>(this.form.value));
  }

  filterForm(value: CapacityFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<CapacityFilter>(value));
  }
}
