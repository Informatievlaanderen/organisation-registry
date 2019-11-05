import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TaskService } from './task.service';

@NgModule({
  declarations: [
  ],
  providers: [
    TaskService
  ],
  exports: [
  ]
})
export class TasksServiceModule { }
