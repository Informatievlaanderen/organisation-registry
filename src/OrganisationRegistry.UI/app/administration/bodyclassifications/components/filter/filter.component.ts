import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { BodyClassificationFilter } from 'services/bodyclassifications';

@Component({
  selector: 'ww-body-classification-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyClassificationFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<BodyClassificationFilter>> = new EventEmitter<SearchEvent<BodyClassificationFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      bodyClassificationTypeName: ['', Validators.nullValidator],
      bodyClassificationTypeId: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name'),
        this.form.get('bodyClassificationTypeName')
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
    this.form.setValue(new BodyClassificationFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: '',
      bodyClassificationTypeName: ''
    });
    this.filter.emit(new SearchEvent<BodyClassificationFilter>(this.form.value));
  }

  filterForm(value: BodyClassificationFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyClassificationFilter>(value));
  }
}
