import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { FormalFrameworkCategoryFilter } from 'services/formalframeworkcategories';

@Component({
  selector: 'ww-formal-framework-category-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class FormalFrameworkCategoryFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<FormalFrameworkCategoryFilter>> = new EventEmitter<SearchEvent<FormalFrameworkCategoryFilter>>();

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
    this.form.setValue(new FormalFrameworkCategoryFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<FormalFrameworkCategoryFilter>(this.form.value));
  }

  filterForm(value: FormalFrameworkCategoryFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<FormalFrameworkCategoryFilter>(value));
  }
}
