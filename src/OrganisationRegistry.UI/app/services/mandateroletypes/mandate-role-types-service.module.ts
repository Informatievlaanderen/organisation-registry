import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MandateRoleTypeService } from './mandate-role-type.service';
import { MandateRoleTypeResolver } from './mandate-role-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    MandateRoleTypeService,
    MandateRoleTypeResolver
  ],
  exports: [
  ]
})
export class MandateRoleTypesServiceModule { }
