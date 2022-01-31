import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { Role, OidcService } from 'core/auth';

@Component({
    templateUrl: 'overview.template.html',
    styleUrls: ['overview.style.css']
})
export class DashboardOverviewComponent implements OnInit {
    public isDeveloper: Observable<boolean>;

    constructor(
        private oidcService: OidcService
    ) { }

    ngOnInit() {
        this.isDeveloper = this.oidcService.roles.map(roles => {
            return (roles.indexOf(Role.Developer) !== -1);
        });
    }
}
