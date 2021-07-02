import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { RegulationTypeFilter } from 'services/regulationtypes';

@Component({
  selector: 'ww-regulation-type-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class RegulationTypeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<RegulationTypeFilter>> = new EventEmitter<SearchEvent<RegulationTypeFilter>>();

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
    this.form.setValue(new RegulationTypeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: ''
    });
    this.filter.emit(new SearchEvent<RegulationTypeFilter>(this.form.value));
  }

  filterForm(value: RegulationTypeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<RegulationTypeFilter>(value));
  }
}
