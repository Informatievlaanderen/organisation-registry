import { AlertMessage, IAlertMessageGroup } from './';

import { ICrudItem } from './../crud';

export class BaseAlertMessages {
    public readonly loadError: AlertMessage;

    constructor(protected typeName: string) {
        this.loadError =
            new AlertMessage(
                `${this.capitalizeFirstChar(typeName).trim()} kon niet geladen worden!`,
                'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
            );
    }

    protected capitalizeFirstChar(text: string) {
        return text.charAt(0).toUpperCase() + text.slice(1);
    }

    protected lowerFirstChar(text: string) {
        return text.charAt(0).toLowerCase() + text.slice(1);
    }
}
