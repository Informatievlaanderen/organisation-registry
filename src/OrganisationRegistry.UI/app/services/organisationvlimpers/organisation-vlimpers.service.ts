import { Injectable } from '@angular/core';
import { Http } from '@angular/http';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

@Injectable()
export class OrganisationVlimpersService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public placeUnderVlimpersManagement(organisationId: string) {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/vlimpers`;

    let headers = new HeadersBuilder()
      .contentJson()
      .build();

    return this.http
      .patch(url, JSON.stringify({'vlimpersManagement': true}), { headers: headers })
      .map(response => response.ok);
  }

  public removeFromVlimpersManagement(organisationId: string) {
    const url = `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/vlimpers`;

    let headers = new HeadersBuilder()
      .contentJson()
      .build();

    return this.http
      .patch(url, JSON.stringify({'vlimpersManagement': false}), { headers: headers })
      .map(response => response.ok);
  }
}
