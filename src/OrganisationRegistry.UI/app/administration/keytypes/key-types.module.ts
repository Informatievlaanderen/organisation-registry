import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { KeyTypesServiceModule } from 'services/keytypes';
import { KeyTypesRoutingModule } from './key-types-routing.module';

import { KeyTypeDetailComponent } from './detail';
import { KeyTypeOverviewComponent } from './overview';

import {
  KeyTypeListComponent,
  KeyTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    KeyTypesRoutingModule,
    KeyTypesServiceModule
  ],
  declarations: [
    KeyTypeDetailComponent,
    KeyTypeListComponent,
    KeyTypeOverviewComponent,
    KeyTypeFilterComponent
  ],
  exports: [
    KeyTypesRoutingModule
  ]
})
export class AdministrationKeyTypesModule { }
