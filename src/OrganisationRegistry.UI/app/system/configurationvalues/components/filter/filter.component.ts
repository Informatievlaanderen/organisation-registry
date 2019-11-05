import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { ConfigurationValueFilter } from 'services/configurationvalues';

@Component({
  selector: 'ww-configuration-value-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class ConfigurationValueFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<ConfigurationValueFilter>> = new EventEmitter<SearchEvent<ConfigurationValueFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      key: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('key')
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
    this.form.setValue(new ConfigurationValueFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      key: ''
    });
    this.filter.emit(new SearchEvent<ConfigurationValueFilter>(this.form.value));
  }

  filterForm(value: ConfigurationValueFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<ConfigurationValueFilter>(value));
  }
}
