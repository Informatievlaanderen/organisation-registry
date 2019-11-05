import { BaseAlertMessages } from './base-alert-messages.model';
import { IAlertMessageGroup } from './alert-message-group.interface';
import { AlertMessage } from './alert-message.model';
import { ICrudItem } from './../crud';

export class CreateAlertMessages extends BaseAlertMessages implements IAlertMessageGroup {
    loadError: AlertMessage;
    saveError: AlertMessage;

    constructor(typeName: string) {
        super(typeName);

        this.saveError =
            new AlertMessage(
                `${this.capitalizeFirstChar(this.typeName).trim()} kon niet bewaard worden!`,
                'Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.'
            );
    }

    saveSuccess(model: ICrudItem<any>): AlertMessage {
        return new AlertMessage(
            `${this.capitalizeFirstChar(this.typeName).trim()} aangemaakt!`,
            `${this.capitalizeFirstChar(this.typeName).trim()} "${model.name.trim()}" is succesvol aangemaakt.`
        );
    }
}
