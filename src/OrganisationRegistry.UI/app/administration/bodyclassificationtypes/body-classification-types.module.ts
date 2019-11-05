import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodyClassificationTypesServiceModule } from 'services/bodyclassificationtypes';
import { BodyClassificationTypesRoutingModule } from './body-classification-types-routing.module';

import { BodyClassificationTypeDetailComponent } from './detail';
import { BodyClassificationTypeOverviewComponent } from './overview';

import {
  BodyClassificationTypeListComponent,
  BodyClassificationTypeFilterComponent
} from './components';

@NgModule({
  imports: [
    SharedModule,
    BodyClassificationTypesRoutingModule,
    BodyClassificationTypesServiceModule
  ],
  declarations: [
    BodyClassificationTypeDetailComponent,
    BodyClassificationTypeListComponent,
    BodyClassificationTypeOverviewComponent,
    BodyClassificationTypeFilterComponent
  ],
  exports: [
    BodyClassificationTypesRoutingModule
  ]
})
export class AdministrationBodyClassificationTypesModule { }
