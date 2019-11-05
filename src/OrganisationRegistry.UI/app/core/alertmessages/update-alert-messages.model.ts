import { BaseAlertMessages } from './base-alert-messages.model';
import { IAlertMessageGroup } from './alert-message-group.interface';
import { AlertMessage } from './alert-message.model';
import { ICrudItem } from './../crud';

export class UpdateAlertMessages extends BaseAlertMessages implements IAlertMessageGroup {
    saveError: AlertMessage;

    constructor(typeName: string) {
        super(typeName);

        this.saveError =
            new AlertMessage(
                `${this.capitalizeFirstChar(this.typeName)} kon niet bijgewerkt worden!`,
                'Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.'
            );
    }

    saveSuccess(model: ICrudItem<any>): AlertMessage {
        return new AlertMessage(
            `${this.capitalizeFirstChar(this.typeName)} bijgewerkt!`,
            `${this.capitalizeFirstChar(this.typeName)} "${model.name}" is succesvol bijgewerkt.`
        );
    }
}
