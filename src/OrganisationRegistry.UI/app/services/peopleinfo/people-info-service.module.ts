import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonInfoService } from './person-info.service';

@NgModule({
  declarations: [
  ],
  providers: [
    PersonInfoService
  ],
  exports: [
  ]
})
export class PeopleInfoServiceModule { }
