import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LabelTypesServiceModule } from 'services/labeltypes';
import { LabelTypesRoutingModule } from './label-types-routing.module';

import { LabelTypeDetailComponent } from './detail';
import { LabelTypeOverviewComponent } from './overview';

import {
  LabelTypeListComponent,
  LabelTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    LabelTypesRoutingModule,
    LabelTypesServiceModule
  ],
  declarations: [
    LabelTypeDetailComponent,
    LabelTypeListComponent,
    LabelTypeOverviewComponent,
    LabelTypeFilterComponent
  ],
  exports: [
    LabelTypesRoutingModule
  ]
})
export class AdministrationLabelTypesModule { }
