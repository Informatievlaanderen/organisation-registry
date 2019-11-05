import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodyClassificationsServiceModule } from 'services/bodyclassifications';
import { BodyClassificationsRoutingModule } from './body-classifications-routing.module';

import { BodyClassificationDetailComponent } from './detail';
import { BodyClassificationOverviewComponent } from './overview';

import {
  BodyClassificationListComponent,
  BodyClassificationFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    BodyClassificationsRoutingModule,
    BodyClassificationsServiceModule
  ],
  declarations: [
    BodyClassificationDetailComponent,
    BodyClassificationListComponent,
    BodyClassificationOverviewComponent,
    BodyClassificationFilterComponent
  ],
  exports: [
    BodyClassificationsRoutingModule
  ]
})
export class AdministrationBodyClassificationsModule { }
