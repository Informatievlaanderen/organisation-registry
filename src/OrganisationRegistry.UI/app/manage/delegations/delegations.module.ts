import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { DelegationsServiceModule } from 'services/delegations';
import { DelegationAssignmentsServiceModule } from 'services/delegationassignments';
import { DelegationsRoutingModule } from './delegations-routing.module';

import { DelegationDetailComponent } from './detail';
import { DelegationOverviewComponent } from './overview';
import { DelegationAssignimentsCreateDelegationAssignmentComponent } from './create';
import { DelegationAssignimentsUpdateDelegationAssignmentComponent } from './update';
import { DelegationAssignimentsDeleteDelegationAssignmentComponent } from './delete';

import {
  DelegationListItemListComponent,
  DelegationFilterComponent,
  DelegationAssignmentComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    DelegationsRoutingModule,
    DelegationsServiceModule,
    DelegationAssignmentsServiceModule
  ],
  declarations: [
    DelegationDetailComponent,
    DelegationListItemListComponent,
    DelegationOverviewComponent,
    DelegationFilterComponent,
    DelegationAssignmentComponent,
    DelegationAssignimentsCreateDelegationAssignmentComponent,
    DelegationAssignimentsUpdateDelegationAssignmentComponent,
    DelegationAssignimentsDeleteDelegationAssignmentComponent
  ],
  exports: [
    DelegationsRoutingModule
  ]
})
export class ManageDelegationsModule { }
