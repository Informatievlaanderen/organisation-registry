import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { EventFilter } from 'services/events';

@Component({
  selector: 'ww-event-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class EventDataFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<EventFilter>> = new EventEmitter<SearchEvent<EventFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      eventNumber: ['', Validators.nullValidator],
      name: ['', Validators.nullValidator],
      firstName: ['', Validators.nullValidator],
      lastName: ['', Validators.nullValidator],
      data: ['', Validators.nullValidator],
      ip: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('eventNumber'),
        this.form.get('name'),
        this.form.get('firstName'),
        this.form.get('lastName'),
        this.form.get('data'),
        this.form.get('ip'),
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
    this.form.setValue(new EventFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      eventNumber: '',
      name: '',
      firstName: '',
      lastName: '',
      data: '',
      ip: ''
    });
    this.filter.emit(new SearchEvent<EventFilter>(this.form.value));
  }

  filterForm(value: EventFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<EventFilter>(value));
  }
}
