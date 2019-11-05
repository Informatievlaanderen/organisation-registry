import { CreateAlertMessages } from 'core/alertmessages';
import { AlertBuilder } from 'core/alert';

import { ICreateOrganisation } from 'services/organisations';

export class CreateChildAlerts {
  private alertMessages = new CreateAlertMessages('Organisatie');

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
