import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { PersonFilter } from 'services/people';

@Component({
  selector: 'ww-person-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class PersonFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<PersonFilter>> = new EventEmitter<SearchEvent<PersonFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      firstName: ['', Validators.nullValidator],
      name: ['', Validators.nullValidator],
      fullName: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('firstName'),
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
    this.form.setValue(new PersonFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      firstName: '',
      name: ''
    });
    this.filter.emit(new SearchEvent<PersonFilter>(this.form.value));
  }

  filterForm(value: PersonFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<PersonFilter>(value));
  }
}
