import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { atLeastOne } from 'core/validation';

import { SearchEvent } from 'core/search';
import { SortOrder } from 'core/pagination';
import { AlertService, AlertBuilder } from 'core/alert';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  OrganisationFilter,
  OrganisationService,
  Organisation
} from 'services/organisations';

import { FormalFrameworkService } from 'services/formalframeworks';
import { OrganisationClassificationService } from 'services/organisationclassifications';
import { OrganisationClassificationTypeService } from 'services/organisationclassificationtypes';

@Component({
  selector: 'ww-organisation-filter',
  templateUrl: 'filter.template.html',
  styleUrls: ['filter.style.css']
})
export class OrganisationFilterComponent implements OnInit {
  @Output() filter: EventEmitter<SearchEvent<OrganisationFilter>> = new EventEmitter<SearchEvent<OrganisationFilter>>();

  public organisationForm: FormGroup;
  public classificationControl: AbstractControl;
  public filterActive: boolean = false;
  public formalFrameworks: Array<SelectItem> = [];
  public classificationTypes: Array<SelectItem> = [];
  public classifications: Array<SelectItem> = [];

  private classificationType: string = '';
  private isBusyInternal: boolean = true;
  private isBusyExternal: boolean = true;

  get isFormValid() {
    return this.organisationForm.enabled && this.organisationForm.valid;
  }

  updateFormEnabled(isBusyInternal, isBusyExternal) {
    this.isBusyInternal = isBusyInternal;
    this.isBusyExternal = isBusyExternal;

    if (this.isBusyInternal || this.isBusyExternal) {
      this.organisationForm.disable();
    } else {
      this.organisationForm.enable();
      // enabling the form re-enables previously disabled controls, including
      // the classificationControl, which possibly needs to be disabled in case
      // there are no classifications.
      this.updateClassificationsEnabled();
    }
  }

  updateClassificationsEnabled() { // todo: we could probably make this logic simpler by creating a classificationControl component.
    if (this.classifications.length > 0) {
      this.classificationControl.enable();
    } else {
      this.classificationControl.disable();
    }
  }

  constructor(
    formBuilder: FormBuilder,
    private organisationService: OrganisationService,
    private formalFrameworkService: FormalFrameworkService,
    private organisationClassificationService: OrganisationClassificationService,
    private organisationClassificationTypeService: OrganisationClassificationTypeService,
    private alertService: AlertService
  ) {
    this.organisationForm = formBuilder.group({
      name: ['', Validators.nullValidator],
      ovoNumber: ['', Validators.nullValidator],
      activeOnly: [true, Validators.nullValidator],
      formalFrameworkId: ['', Validators.nullValidator],
      organisationClassificationTypeId: ['', Validators.nullValidator],
      organisationClassificationId: ['', Validators.nullValidator],
      authorizedOnly: [false, Validators.nullValidator]
    });

    this.classificationControl = this.organisationForm.controls['organisationClassificationId'];
  }

  @Input('isBusy')
  set name(isBusy: boolean) {
    this.updateFormEnabled(this.isBusyInternal, isBusy);
  }

  ngOnInit() {
    this.subcribeToFormChanges();
    this.populateSelects();
    this.organisationForm.setValue(new OrganisationFilter());
  }

  populateSelects() {
    this.updateFormEnabled(true, this.isBusyExternal);

    // TODO: Combine these 2 observables to only set isBusyInternal when both are done

    this.formalFrameworkService
      .getAllFormalFrameworks()
      .finally(() => this.updateFormEnabled(false, this.isBusyExternal))
      .subscribe(
          allKeys => this.formalFrameworks = allKeys.map(k => new SelectItem(k.id, k.name)),
          error =>
              this.alertService.setAlert(
                  new AlertBuilder()
                      .error(error)
                      .withTitle('Toepassingsgebieden konden niet geladen worden!')
                      .withMessage('Er is een fout opgetreden bij het ophalen van de toepassingsgebieden. Probeer het later opnieuw.')
                      .build()));

    this.organisationClassificationTypeService
      .getAllOrganisationClassificationTypes()
      .finally(() => this.updateFormEnabled(false, this.isBusyExternal))
      .subscribe(
          allKeys => this.classificationTypes = allKeys.map(k => new SelectItem(k.id, k.name)),
          error =>
              this.alertService.setAlert(
                  new AlertBuilder()
                      .error(error)
                      .withTitle('Classificatietypes konden niet geladen worden!')
                      .withMessage('Er is een fout opgetreden bij het ophalen van de classificatietypes. Probeer het later opnieuw.')
                .build()));
  }

  subcribeToFormChanges() {
    const classificationTypeChanges$ = this.organisationForm.controls['organisationClassificationTypeId'].valueChanges;

    classificationTypeChanges$
      .subscribe(classificationType => {
        if (this.classificationType === classificationType)
          return;

        this.classificationType = classificationType;

        this.organisationForm.patchValue({ organisationClassificationId: '' });

        if (classificationType) {
          this.updateFormEnabled(true, this.isBusyExternal);

          this.organisationClassificationService
            .getAllOrganisationClassifications(classificationType)
            .finally(() => this.updateFormEnabled(false, this.isBusyExternal))
            .subscribe(
              allClassifications => {
                this.classifications = allClassifications.map(c => new SelectItem(c.id, c.name));
                this.updateClassificationsEnabled();
              },
              error =>
                this.alertService.setAlert(
                  new AlertBuilder()
                    .error(error)
                    .withTitle('Classificaties konden niet geladen worden!')
                    .withMessage('Er is een fout opgetreden bij het ophalen van de classificaties. Probeer het later opnieuw.')
                    .build()));
        } else {
          this.classificationControl.disable();
        }
      });
  }

  resetForm() {
    this.filterActive = false;
    this.organisationForm.reset({
      name: '',
      ovoNumber: '',
      activeOnly: true,
      formalFrameworkId: '',
      organisationClassificationTypeId: '',
      organisationClassificationId: '',
      authorizedOnly: false
    });

    this.filter.emit(new SearchEvent<OrganisationFilter>(this.organisationForm.value));
  }

  filterForm(value: OrganisationFilter) {
    this.filterActive = true;
    this.filter.emit(new SearchEvent<OrganisationFilter>(value));
  }
}
