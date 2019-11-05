import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { SearchEvent } from 'core/search';
import { AlertService, AlertBuilder } from 'core/alert';

import {
  BodyBodyClassificationFilter,
  BodyBodyClassificationService
} from 'services/bodybodyclassifications';

@Component({
  selector: 'ww-body-bodyclassification-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class BodyBodyClassificationsFilterComponent implements OnInit {
  @Input('isBusy') isBusyExternal: boolean = true;
  @Output() filter: EventEmitter<SearchEvent<BodyBodyClassificationFilter>> = new EventEmitter<SearchEvent<BodyBodyClassificationFilter>>();

  public bodyBodyClassificationForm: FormGroup;
  public filterActive: boolean = false;

  private isBusyInternal: boolean = false;

  get isBusy() {
    return this.isBusyExternal || this.isBusyInternal;
  }

  get isFormValid() {
    return !this.isBusy && this.bodyBodyClassificationForm.valid;
  }

  constructor(
    formBuilder: FormBuilder,
    private bodyBodyClassificationService: BodyBodyClassificationService,
    private alertService: AlertService
  ) {
    this.bodyBodyClassificationForm = formBuilder.group({
      showHistory: [false, Validators.nullValidator]
    });
  }

  ngOnInit() {
    this.bodyBodyClassificationForm.setValue({
      showHistory: false
    });
  }

  resetForm() {
    this.filterActive = false;
    this.bodyBodyClassificationForm.reset({
      showHistory: false
    });

    this.filter.emit(new SearchEvent<BodyBodyClassificationFilter>(new BodyBodyClassificationFilter(!this.bodyBodyClassificationForm.value.showHistory)));
  }

  filterForm(value) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<BodyBodyClassificationFilter>(new BodyBodyClassificationFilter(!value.showHistory)));
  }
}
