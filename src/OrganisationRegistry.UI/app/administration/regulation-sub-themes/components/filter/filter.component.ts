import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { RegulationSubThemeFilter } from 'services/regulation-sub-themes';

@Component({
  selector: 'ww-regulation-sub-theme-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class RegulationSubThemeFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<RegulationSubThemeFilter>> = new EventEmitter<SearchEvent<RegulationSubThemeFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      regulationThemeName: ['', Validators.nullValidator],
      regulationThemeId: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name'),
        this.form.get('regulationThemeName')
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
    this.form.setValue(new RegulationSubThemeFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: '',
      regulationThemeName: ''
    });
    this.filter.emit(new SearchEvent<RegulationSubThemeFilter>(this.form.value));
  }

  filterForm(value: RegulationSubThemeFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<RegulationSubThemeFilter>(value));
  }
}
