import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { DelegationFilter } from 'services/delegations';

@Component({
  selector: 'ww-delegation-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class DelegationFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<DelegationFilter>> = new EventEmitter<SearchEvent<DelegationFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      bodyName: ['', Validators.nullValidator],
      bodyOrganisationName: ['', Validators.nullValidator],
      organisationName: ['', Validators.nullValidator],
      functionTypeName: ['', Validators.nullValidator],
      bodySeatName: ['', Validators.nullValidator],
      bodySeatNumber: ['', Validators.nullValidator],
      activeMandatesOnly: [true, Validators.nullValidator],
      emptyDelegationsOnly: [false, Validators.nullValidator]
    });
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
    this.form.setValue(new DelegationFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      bodyName: '',
      bodyOrganisationName: '',
      organisationName: '',
      functionTypeName: '',
      bodySeatName: '',
      bodySeatNumber: '',
      activeMandatesOnly: true,
      emptyDelegationsOnly: false
    });
    this.filter.emit(new SearchEvent<DelegationFilter>(this.form.value));
  }

  filterForm(value: DelegationFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<DelegationFilter>(value));
  }
}
