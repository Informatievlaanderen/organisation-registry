import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { SearchEvent } from 'core/search';
import { AlertService, AlertBuilder } from 'core/alert';

import {
  BodyParticipationReportFilter,
  BodyParticipationReportService,
  BodyParticipationReportListItem
} from 'services/reports/body-participation';

@Component({
    selector: 'ww-body-participation-filter',
    templateUrl: 'filter.template.html',
    styleUrls: ['filter.style.css']
  })
  export class BodyParticipationFilterComponent implements OnInit {
    @Input('isBusy') isBusyExternal: boolean = true;
    @Output() filter: EventEmitter<SearchEvent<BodyParticipationReportFilter>> = new EventEmitter<SearchEvent<BodyParticipationReportFilter>>();

    public bodyParticipationForm: FormGroup;
    public filterActive: boolean = false;

    private isBusyInternal: boolean = false;

    get isBusy() {
      return this.isBusyExternal || this.isBusyInternal;
    }

    get isFormValid() {
      return !this.isBusy && this.bodyParticipationForm.valid;
    }

    constructor(
      formBuilder: FormBuilder,
      private bodyFormalFrameworkService: BodyParticipationReportService,
      private alertService: AlertService
    ) {
      this.bodyParticipationForm = formBuilder.group({
        entitledToVote: [true, Validators.nullValidator],
        notEntitledToVote: [false, Validators.nullValidator]
      });
    }

    ngOnInit() {
      this.bodyParticipationForm.setValue({
        entitledToVote: true,
        notEntitledToVote: false
      });
    }

    resetForm() {
      this.filterActive = false;
      this.bodyParticipationForm.reset({
        entitledToVote: true,
        notEntitledToVote: false
      });

      this.filter.emit(
        new SearchEvent<BodyParticipationReportFilter>(
          new BodyParticipationReportFilter(
            this.bodyParticipationForm.value.entitledToVote,
            this.bodyParticipationForm.value.notEntitledToVote)));
    }

    filterForm(value) {
      this.filterActive = true;

      this.filter.emit(
        new SearchEvent<BodyParticipationReportFilter>(
          new BodyParticipationReportFilter(
            value.entitledToVote,
            value.notEntitledToVote)));
    }
  }
