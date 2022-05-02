import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { KeyTypeFilter } from 'services/keytypes';

@Component({
  selector: 'ww-key-type-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class KeyTypeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<KeyTypeFilter>> = new EventEmitter<SearchEvent<KeyTypeFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      showAll: [true],
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
    this.form.setValue(new KeyTypeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<KeyTypeFilter>(this.form.value));
  }

  filterForm(value: KeyTypeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<KeyTypeFilter>(value));
  }
}
