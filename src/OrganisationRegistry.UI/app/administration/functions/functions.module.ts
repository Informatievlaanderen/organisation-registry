import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { FunctionsServiceModule } from 'services/functions';
import { FunctionsRoutingModule } from './functions-routing.module';

import { FunctionDetailComponent } from './detail';
import { FunctionOverviewComponent } from './overview';

import {
  FunctionListComponent,
  FunctionFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    FunctionsRoutingModule,
    FunctionsServiceModule
  ],
  declarations: [
    FunctionDetailComponent,
    FunctionListComponent,
    FunctionOverviewComponent,
    FunctionFilterComponent
  ],
  exports: [
    FunctionsRoutingModule
  ]
})
export class AdministrationFunctionsModule { }
