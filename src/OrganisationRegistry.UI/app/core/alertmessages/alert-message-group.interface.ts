import { AlertMessage } from './alert-message.model';
import { ICrudItem } from './../crud';

export interface IAlertMessageGroup {
    readonly loadError: AlertMessage;
    readonly saveError: AlertMessage;
    saveSuccess(model: ICrudItem<any>): AlertMessage;
}
