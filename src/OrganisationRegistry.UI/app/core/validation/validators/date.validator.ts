import { AbstractControl, FormGroup } from '@angular/forms';

declare var moment: any; // global moment

export function date(control: AbstractControl): { [key: string]: boolean } {
    if (control == null || control.value == null || control.value.length === 0)
        return null;

    let pattern = /^\d{4}\-\d{2}\-\d{2}$/; // ####-##-##
    let stringvalue = control.value.toString();

    if (!stringvalue.match(pattern))
        return { 'invalidDate': true };

    let validDotNetDate = moment(stringvalue, 'YYYY-MM-DD').isSameOrAfter(moment('0001-01-01', 'YYYY-MM-DD'));

    return validDotNetDate ? null : { 'invalidDate': true };
}
