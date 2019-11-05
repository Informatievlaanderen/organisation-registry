import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { FormalFrameworkFilter } from 'services/formalframeworks';

@Component({
  selector: 'ww-report-formal-framework-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class FormalFrameworkFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<FormalFrameworkFilter>> = new EventEmitter<SearchEvent<FormalFrameworkFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      code: ['', Validators.nullValidator],
      formalFrameworkCategoryName: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name'),
        this.form.get('code'),
        this.form.get('formalFrameworkCategoryName')
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
    this.form.setValue(new FormalFrameworkFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: '',
      code: '',
      formalFrameworkCategoryName: ''
    });
    this.filter.emit(new SearchEvent<FormalFrameworkFilter>(this.form.value));
  }

  filterForm(value: FormalFrameworkFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<FormalFrameworkFilter>(value));
  }
}
