import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';
import { OrganisationsServiceModule } from 'services/organisations';

import { BodyRoutingModule } from './body-routing.module';

import { BodyOverviewComponent, BodyListComponent, BodyFilterComponent } from './overview';
import { CreateBodyComponent, CreateBodyFormComponent } from './create';
import { BodyDetailModule } from './detail';

@NgModule({
  imports: [
    SharedModule,
    BodyRoutingModule,
    BodiesServiceModule,
    OrganisationsServiceModule,
    BodyDetailModule
  ],
  declarations: [
    BodyOverviewComponent,
    BodyListComponent,
    BodyFilterComponent,
    CreateBodyComponent,
    CreateBodyFormComponent
  ],
  exports: [
    BodyRoutingModule
  ]
})
export class BodyModule { }
