import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

@Injectable()
export class ManagementService  {
  private managementUrl = `${this.configurationService.apiUrl}/v1/mgmt`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllDays() {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(`${this.managementUrl}/days`, { headers: headers })
      .map(res => res.json());
  }

  public getAllHours() {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(`${this.managementUrl}/hours`, { headers: headers })
      .map(res => res.json());
  }
}
