import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { DashboardOverviewComponent } from './overview';

const routes: Routes = [
    {
        path: 'report', 
        component: DashboardOverviewComponent,
        data: {
            title: 'Rapportering'
        }
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [
        RouterModule
    ]
})
export class DashboardRoutingModule { }
