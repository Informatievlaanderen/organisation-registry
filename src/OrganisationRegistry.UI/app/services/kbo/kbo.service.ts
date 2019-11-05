import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Kbo
} from './';

@Injectable()
export class KboService {
  private kboUrl = `${this.configurationService.apiUrl}/v1/kbo`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public get(kboNumber: string): Observable<Kbo> {
    const url = `${this.kboUrl}/${kboNumber}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toKbo);
  }

  private toKbo(res: Response): Kbo {
    let body = res.json();
    return (body || {}) as Kbo;
  }
}
