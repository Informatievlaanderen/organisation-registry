import { Observable } from 'rxjs/Observable';

import { AlertBuilder, AlertService } from './../../core/alert';
import { IAlertMessageGroup } from './../alertmessages/';

import { ICrud } from './crud.interface';
import { ICrudItem } from './crud-item.interface';
import { ICrudService } from './crud-service.interface';

export class Update<TService extends ICrudService<T>, T extends ICrudItem<T>> implements ICrud<T> {
  constructor(
    private itemId: string,
    private itemService: TService,
    private alertService: AlertService,
    private alertMessages: IAlertMessageGroup
  ) { }

  // TODO: can't we just pass the object here as a param?
  // There is an awful lot of duplication with create.class.ts
  load(): Observable<T> {
    return this.itemService.get(this.itemId);
  }

  save(item: T): Observable<string> {
    return this.itemService.update(item);
  }

  alertLoadError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle(this.alertMessages.loadError.title)
        .withMessage(this.alertMessages.loadError.message)
        .build());
  }

  alertSaveSuccess(model: T, itemUrl: string) {
    let alert = this.alertMessages.saveSuccess(model);
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle(alert.title)
        .withMessage(alert.message)
        .linkTo(itemUrl)
        .build());
  }

  alertSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle(this.alertMessages.saveError.title)
        .withMessage(this.alertMessages.saveError.message)
        .build());
  }
}
