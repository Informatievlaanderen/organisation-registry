import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { Person, PersonService } from 'services/people';

import { AlertBuilder, AlertService, Alert, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';

@Injectable()
export class PersonInfoService {
  private personChangedSource: Subject<Person>;
  private personChanged$: Observable<Person>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon');

  constructor(
    private personService: PersonService,
    private alertService: AlertService
  ) {
    this.personChangedSource = new Subject<Person>();
    this.personChanged$ = this.personChangedSource.asObservable();
  }

  get personChanged() {
    return this.personChanged$;
  }

  loadPerson(id: string) {
    this.personService.get(id)
      .subscribe(
        item => {
          if (item)
            this.personChangedSource.next(item);
        },
        error => this.alertLoadError(error));
  }

  private alertLoadError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle(this.alertMessages.loadError.title)
        .withMessage(this.alertMessages.loadError.message)
        .build());
  }
}
