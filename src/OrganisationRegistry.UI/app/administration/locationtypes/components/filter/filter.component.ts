import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { LocationTypeFilter } from 'services/locationtypes';

@Component({
  selector: 'ww-location-type-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class LocationTypeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<LocationTypeFilter>> = new EventEmitter<SearchEvent<LocationTypeFilter>>();

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
    this.form.setValue(new LocationTypeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<LocationTypeFilter>(this.form.value));
  }

  filterForm(value: LocationTypeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<LocationTypeFilter>(value));
  }
}
