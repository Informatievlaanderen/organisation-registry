import { Injectable } from '@angular/core';

import { Alert } from './alert.model';

@Injectable()
export class AlertService {
    private alert: Alert = undefined;

    public get hasAlerts(): boolean {
        return this.alert !== undefined;
    }

    public get activeAlert(): Alert {
        return this.alert;
    }

    public setAlert(alert: Alert) {
        this.alert = alert;
    }

    public clearAlert() {
        this.alert = undefined;
    }
}
