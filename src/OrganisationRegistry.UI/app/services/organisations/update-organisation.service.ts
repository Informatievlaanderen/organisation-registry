import { Response } from '@angular/http';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Organisation,
  UpdateOrganisationRequest,
} from './';

@Injectable()
export class UpdateOrganisationService {
  private organisationsUrl = `${this.configurationService.apiUrl}/v1/organisations`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public get(id: string): Observable<Organisation> {
    const url = `${this.organisationsUrl}/${id}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisation);
  }

  public update(organisationId, organisation: UpdateOrganisationRequest): Observable<string> {
    const url = `${this.organisationsUrl}/${organisationId}`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public updateInfoNotLimitedToVlimpers(organisationId, organisation: UpdateOrganisationRequest): Observable<string> {
    const url = `${this.organisationsUrl}/${organisationId}/notlimitedtovlimpers`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  public updateInfoLimitedToVlimpers(organisationId, organisation: UpdateOrganisationRequest): Observable<string> {
    const url = `${this.organisationsUrl}/${organisationId}/limitedtovlimpers`;

    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .put(url, JSON.stringify(organisation), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }

  private toOrganisation(res: Response): Organisation {
    let body = res.json();
    return (body || {}) as Organisation;
  }
}
