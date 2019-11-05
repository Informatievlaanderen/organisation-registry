import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { LocationFilter } from 'services/locations';

@Component({
  selector: 'ww-location-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class LocationFilterComponent implements OnInit {
  @Output() search: EventEmitter<SearchEvent<LocationFilter>> = new EventEmitter<SearchEvent<LocationFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      street: [''],
      zipCode: [''],
      city: [''],
      country: [''],
      nonCrabOnly: [false]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('street'),
        this.form.get('zipCode'),
        this.form.get('city'),
        this.form.get('country'),
        this.form.get('nonCrabOnly')
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
    this.form.setValue(new LocationFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      street: '',
      zipCode: '',
      city: '',
      country: '',
      nonCrabOnly: false
    });
    this.search.emit(new SearchEvent<LocationFilter>(this.form.value));
  }

  filterForm(value: LocationFilter) {
    this.filterActive = true;
    this.search.emit(new SearchEvent<LocationFilter>(value));
  }
}
