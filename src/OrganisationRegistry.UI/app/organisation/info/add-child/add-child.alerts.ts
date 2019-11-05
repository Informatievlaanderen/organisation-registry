import { CreateAlertMessages, IAlertMessageGroup } from 'core/alertmessages';
import { AlertBuilder } from 'core/alert';

import { ICreateOrganisation, Organisation } from 'services/organisations';

export class AddChildAlerts {
  private alertMessages = new CreateAlertMessages('Dochterorganisatie');

  loadError(error) {
    new AlertBuilder()
      .error(error)
      .withTitle(this.alertMessages.loadError.title)
      .withMessage(this.alertMessages.loadError.message)
      .build();
  }

  saveSuccess(model: ICreateOrganisation, itemUrl: string) {
    let alert = this.alertMessages.saveSuccess(model);
    return new AlertBuilder()
      .success()
      .withTitle(alert.title)
      .withMessage(alert.message)
      .linkTo(itemUrl)
      .build();
  }

  saveError(error) {
    return new AlertBuilder()
      .error(error)
      .withTitle(this.alertMessages.saveError.title)
      .withMessage(this.alertMessages.saveError.message)
      .build();
  }
}
