import { Component } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { OidcService } from 'core/auth';
import { ConfigurationService } from 'core/configuration';

@Component({
  templateUrl: 'callback.template.html',
  styleUrls: ['callback.style.css']
})
export class CallbackComponent {
  constructor(
    private route: ActivatedRoute,
    private configurationService: ConfigurationService,
    private oidcService: OidcService,
  ) { }

  ngOnInit() {
    this.route.queryParams.subscribe((params: Params) => {
      this.oidcService.exchangeCode(params['code'], this.configurationService);
    });
  }
}
