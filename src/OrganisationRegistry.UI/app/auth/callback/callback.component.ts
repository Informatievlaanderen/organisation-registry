import {Component, OnDestroy} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { OidcService } from 'core/auth';
import { ConfigurationService } from 'core/configuration';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'callback.template.html',
  styleUrls: ['callback.style.css']
})
export class CallbackComponent implements OnDestroy {

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private configurationService: ConfigurationService,
    private oidcService: OidcService,
  ) { }

  ngOnInit() {
    this.subscriptions.push(this.route.queryParams.subscribe((params: Params) => {
      this.oidcService.exchangeCode(params['code'], this.configurationService);
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
