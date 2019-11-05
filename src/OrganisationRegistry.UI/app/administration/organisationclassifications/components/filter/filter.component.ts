import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';
import { SearchEvent } from 'core/search';

import { OrganisationClassificationFilter } from 'services/organisationclassifications';

@Component({
  selector: 'ww-organisation-classification-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationClassificationFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<OrganisationClassificationFilter>> = new EventEmitter<SearchEvent<OrganisationClassificationFilter>>();

  public form: FormGroup;
  public filterActive: boolean = false;

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  constructor(formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      name: ['', Validators.nullValidator],
      organisationClassificationTypeName: ['', Validators.nullValidator],
      organisationClassificationTypeId: ['', Validators.nullValidator]
    });
    this.form.setValidators(
      atLeastOne(
        this.form.get('name'),
        this.form.get('organisationClassificationTypeName')
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
    this.form.setValue(new OrganisationClassificationFilter());
  }

  resetForm() {
    this.filterActive = false;
    this.form.reset({
      name: '',
      organisationClassificationTypeName: ''
    });
    this.filter.emit(new SearchEvent<OrganisationClassificationFilter>(this.form.value));
  }

  filterForm(value: OrganisationClassificationFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationClassificationFilter>(value));
  }
}
