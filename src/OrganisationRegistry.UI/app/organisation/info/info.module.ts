import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PurposesServiceModule } from 'services/purposes';

import { OrganisationInfoRoutingModule } from './info-routing.module';

import { OrganisationInfoComponent } from './info.component';
import { OrganisationInfoOverviewComponent, OrganisationInfoChildrenComponent } from './overview';
import { OrganisationInfoEditComponent } from './edit';
import { OrganisationInfoAddChildOrganisationComponent, CreateChildOrganisationFormComponent } from './add-child';
import { OrganisationCoupleWithKboComponent } from './couple-with-kbo';
import { OrganisationCancelCouplingWithKboComponent } from './cancel-coupling-with-kbo';
import { OrganisationTerminateComponent } from './terminate';

@NgModule({
  imports: [
    SharedModule,
    PurposesServiceModule,
    OrganisationInfoRoutingModule,
  ],
  declarations: [
    OrganisationInfoComponent,
    OrganisationInfoOverviewComponent,
    OrganisationInfoChildrenComponent,
    OrganisationInfoEditComponent,
    OrganisationInfoAddChildOrganisationComponent,
    CreateChildOrganisationFormComponent,
    OrganisationCoupleWithKboComponent,
    OrganisationCancelCouplingWithKboComponent,
    OrganisationTerminateComponent
  ],
  exports: [
    OrganisationInfoRoutingModule
  ]
})
export class OrganisationInfoModule { }
