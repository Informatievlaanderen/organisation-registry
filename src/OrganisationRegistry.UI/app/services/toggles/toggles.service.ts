import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Toggles
} from './';

@Injectable()
export class TogglesService  {
  private togglesUrl = `${this.configurationService.apiUrl}/v1/status/toggles`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllToggles(): Observable<Toggles> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.togglesUrl, { headers: headers })
      .map(this.toToggles);
  }

  private toToggles(res: Response): Toggles {
    let body = res.json();
    return body || {} as Toggles;
  }
}
