import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { BuildingFilter } from 'services/buildings';

@Component({
  selector: 'ww-building-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BuildingFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<BuildingFilter>> = new EventEmitter<SearchEvent<BuildingFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      vimId: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name'),
        this.form.get('vimId')
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
    this.form.setValue(new BuildingFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: '',
      vimId: ''
    });
    this.filter.emit(new SearchEvent<BuildingFilter>(this.form.value));
  }

  filterForm(value: BuildingFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BuildingFilter>(value));
  }
}
