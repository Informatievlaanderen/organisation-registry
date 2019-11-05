import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodyClassificationsServiceModule } from 'services/bodyclassifications';
import { BodyClassificationTypesServiceModule } from 'services/bodyclassificationtypes';
import { BodyBodyClassificationsServiceModule } from 'services/bodybodyclassifications';
import { BodyBodyClassificationsRoutingModule } from './classifications-routing.module';
import { BodyBodyClassificationsComponent } from './classifications.component';
import { BodyBodyClassificationsOverviewComponent } from './overview';
import { BodyBodyClassificationsListComponent } from './list';
import { BodyBodyClassificationsFilterComponent } from './filter';
import { BodyBodyClassificationsCreateBodyBodyClassificationComponent } from './create';
import { BodyBodyClassificationsUpdateBodyBodyClassificationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    BodyBodyClassificationsRoutingModule,
    BodyBodyClassificationsServiceModule,
    BodyClassificationsServiceModule,
    BodyClassificationTypesServiceModule
  ],
  declarations: [
    BodyBodyClassificationsComponent,
    BodyBodyClassificationsOverviewComponent,
    BodyBodyClassificationsListComponent,
    BodyBodyClassificationsFilterComponent,
    BodyBodyClassificationsCreateBodyBodyClassificationComponent,
    BodyBodyClassificationsUpdateBodyBodyClassificationComponent
  ],
  exports: [
    BodyBodyClassificationsRoutingModule
  ]
})
export class BodyClassificationsModule { }
