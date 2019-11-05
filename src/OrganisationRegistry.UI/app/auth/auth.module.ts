import { NgModule } from '@angular/core';
import { AuthRoutingModule } from './auth-routing.module';
import { CallbackComponent } from './callback';

@NgModule({
  imports: [
  ],
  declarations: [
    CallbackComponent
  ],
  exports: [
    AuthRoutingModule
  ]
})
export class AuthModule { }
