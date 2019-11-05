import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';

import { BodyFormalFrameworksServiceModule } from 'services/bodyformalframeworks';
import { BodyFormalFrameworksOverviewComponent } from './overview';
import { BodyFormalFrameworksListComponent } from './list';
import { BodyFormalFrameworksFilterComponent } from './filter';
import { BodyFormalFrameworksComponent } from './formalframeworks.component';
import { BodyFormalFrameworksRoutingModule } from './formalframeworks-routing.module';
import { BodyFormalFrameworksCreateBodyFormalFrameworkComponent } from './create';
import { BodyFormalFrameworksUpdateBodyFormalFrameworkComponent } from './update';


@NgModule({
  imports: [
    SharedModule,
    BodyFormalFrameworksRoutingModule,
    BodyFormalFrameworksServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyFormalFrameworksComponent,
    BodyFormalFrameworksOverviewComponent,
    BodyFormalFrameworksListComponent,
    BodyFormalFrameworksFilterComponent,BodyFormalFrameworksFilterComponent,
    BodyFormalFrameworksCreateBodyFormalFrameworkComponent,
    BodyFormalFrameworksUpdateBodyFormalFrameworkComponent
  ],
  exports: [
    BodyFormalFrameworksRoutingModule
  ]
})
export class BodyFormalFrameworksModule { }
