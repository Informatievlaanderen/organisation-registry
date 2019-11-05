import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ContactTypeService } from './contact-type.service';
import { ContactTypeResolver } from './contact-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    ContactTypeService,
    ContactTypeResolver
  ],
  exports: [
  ]
})
export class ContactTypesServiceModule { }
