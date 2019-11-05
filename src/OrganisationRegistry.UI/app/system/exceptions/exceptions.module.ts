import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ExceptionsServiceModule } from 'services/exceptions';
import { ExceptionsRoutingModule } from './exceptions-routing.module';

import { ExceptionOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    ExceptionsRoutingModule,
    ExceptionsServiceModule
  ],
  declarations: [
    ExceptionOverviewComponent
  ],
  exports: [
    ExceptionsRoutingModule
  ]
})
export class SystemExceptionsModule { }
