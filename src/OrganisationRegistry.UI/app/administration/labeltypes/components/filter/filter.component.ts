import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { LabelTypeFilter } from 'services/labeltypes';

@Component({
  selector: 'ww-label-type-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class LabelTypeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<LabelTypeFilter>> = new EventEmitter<SearchEvent<LabelTypeFilter>>();

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
    this.form.setValue(new LabelTypeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<LabelTypeFilter>(this.form.value));
  }

  filterForm(value: LabelTypeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<LabelTypeFilter>(value));
  }
}
