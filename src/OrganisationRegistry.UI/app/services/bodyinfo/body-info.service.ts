import {Injectable, OnDestroy} from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

import { Body, BodyService } from 'services/bodies';

import { AlertBuilder, AlertService, Alert, AlertType } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';
import {Subscription} from "rxjs/Subscription";

@Injectable()
export class BodyInfoService implements OnDestroy {
  private bodyChangedSource: Subject<Body>;
  private bodyChanged$: Observable<Body>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Orgaan');

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private bodyService: BodyService,
    private alertService: AlertService
  ) {
    this.bodyChangedSource = new Subject<Body>();
    this.bodyChanged$ = this.bodyChangedSource.asObservable();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get bodyChanged() {
    return this.bodyChanged$;
  }

  loadBody(id: string) {
    this.subscriptions.push(this.bodyService.get(id)
      .subscribe(
        item => {
          if (item)
            this.bodyChangedSource.next(item);
        },
        error => this.alertLoadError(error)));
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
