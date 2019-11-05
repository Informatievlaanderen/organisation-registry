import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import { TaskData } from './';

@Injectable()
export class TaskService {
  private tasksUrl = `${this.configurationService.apiUrl}/v1/tasks`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public submit(taskData: TaskData): Observable<string> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .post(this.tasksUrl, JSON.stringify(taskData), { headers: headers })
      .map(this.getLocationHeader);
  }

  private getLocationHeader(res: Response): string {
    return res.headers.get('Location');
  }
}
