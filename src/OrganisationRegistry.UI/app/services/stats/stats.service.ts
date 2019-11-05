import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Stats
} from './';

@Injectable()
export class StatsService  {
  private statsUrl = `${this.configurationService.apiUrl}/v1/status/stats`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllStats(daysBack: number = 7): Observable<Stats> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.statsUrl + `?daysBack=${daysBack}`, { headers: headers })
      .map(this.toStats);
  }

  private toStats(res: Response): Stats {
    let body = res.json();
    return body || {} as Stats;
  }
}
