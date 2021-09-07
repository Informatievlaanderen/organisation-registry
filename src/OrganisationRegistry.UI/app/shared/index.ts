import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';

import { TogglesServiceModule } from 'services/toggles';

import { FormGroupAutocomplete } from './components/form/form-group-autocomplete';
import { FormGroupDatepicker } from './components/form/form-group-datepicker';
import { EnvironmentWarningComponent } from './components/environment-warning';
import { FormGroupDatepickerRange } from './components/form/form-group-datepicker-range';
import { FormGroupSelect } from './components/form/form-group-select';
import { FormGroupTextbox } from './components/form/form-group-textbox';
import { FormGroupTaglist } from './components/form/form-group-taglist';
import { FormGroupToggle } from './components/form/form-group-toggle';
import { FormGroupTextArea } from './components/form/form-group-textarea';
import { FormGroupRadio } from './components/form/form-group-radio';
import { NavbarComponent } from './components/navbar';
import { HeaderComponent } from './components/header';
import { SmartLinkComponent } from './components/smart-link';

import {
  BodyClassificationTypeAutoComplete,
  FormalFrameworkCategoryAutoComplete,
  FormalFrameworkAutoComplete,
  OrganisationClassificationTypeAutoComplete,
  PersonAutoComplete,
  OrganisationAutoComplete,
  BuildingAutoComplete,
  LocationAutoComplete,
  LocationTypeAutoComplete,
  CrabLocationAutoComplete
} from './components/autocomplete';

import { PopupDirective } from './components/popup';
import { DatepickerDirective } from './components/datepicker';

import { KeysPipe } from './pipes/keys';
import { BaseListComponent } from 'shared/components/list';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    TogglesServiceModule
  ],
  declarations: [
    DatepickerDirective,
    FormGroupAutocomplete,
    FormGroupDatepicker,
    EnvironmentWarningComponent,
    FormGroupDatepickerRange,
    FormGroupSelect,
    FormGroupTextbox,
    FormGroupToggle,
    FormGroupTextArea,
    FormGroupTaglist,
    FormGroupRadio,
    NavbarComponent,
    HeaderComponent,
    PopupDirective,
    BodyClassificationTypeAutoComplete,
    FormalFrameworkCategoryAutoComplete,
    FormalFrameworkAutoComplete,
    OrganisationClassificationTypeAutoComplete,
    OrganisationAutoComplete,
    PersonAutoComplete,
    BuildingAutoComplete,
    LocationAutoComplete,
    LocationTypeAutoComplete,
    CrabLocationAutoComplete,
    SmartLinkComponent,
    KeysPipe,
    BaseListComponent,
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    DatepickerDirective,
    FormGroupAutocomplete,
    FormGroupDatepicker,
    EnvironmentWarningComponent,
    FormGroupDatepickerRange,
    FormGroupSelect,
    FormGroupTextbox,
    FormGroupToggle,
    FormGroupTextArea,
    FormGroupTaglist,
    FormGroupRadio,
    NavbarComponent,
    HeaderComponent,
    PopupDirective,
    BodyClassificationTypeAutoComplete,
    FormalFrameworkCategoryAutoComplete,
    FormalFrameworkAutoComplete,
    OrganisationClassificationTypeAutoComplete,
    OrganisationAutoComplete,
    PersonAutoComplete,
    BuildingAutoComplete,
    LocationAutoComplete,
    LocationTypeAutoComplete,
    CrabLocationAutoComplete,
    SmartLinkComponent,
    KeysPipe
  ]
})
export class SharedModule { }

// TODO: If ever have providers: http://blog.angular-university.io/angular2-ngmodule/#sharedmodulesandlazyloading
