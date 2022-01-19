import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { EventFilter } from 'services/events';
import {Observable} from "rxjs/Observable";

@Component({
  selector: 'ww-event-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class EventDataFilterComponent implements OnInit {
  @Output() onFilter: EventEmitter<SearchEvent<EventFilter>> = new EventEmitter<SearchEvent<EventFilter>>();
  @Input() filterChanged: Observable<EventFilter>;

  public form: FormGroup;
  public filterActive: boolean = false;

  private filter: EventFilter;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      aggregateId: ['', Validators.nullValidator],
      eventNumber: ['', Validators.nullValidator],
      name: ['', Validators.nullValidator],
      firstName: ['', Validators.nullValidator],
      lastName: ['', Validators.nullValidator],
      data: ['', Validators.nullValidator],
      ip: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('aggregateId'),
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
    this.filterChanged.subscribe((filter)=> {
      this.form.setValue(filter);
    });
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      aggregateId: '',
      eventNumber: '',
      name: '',
      firstName: '',
      lastName: '',
      data: '',
      ip: ''
    });
    this.onFilter.emit(new SearchEvent<EventFilter>(this.form.value));
  }

  filterForm(value: EventFilter) {
    this.filterActive = true;
    this.onFilter.emit(new SearchEvent<EventFilter>(value));
  }
}
