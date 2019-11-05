import { AbstractControl, FormGroup } from '@angular/forms';

export function optionalNumber(control: AbstractControl): { [key: string]: boolean } {
    if (control == null || control.value == null || control.value.length === 0)
        return null;

    let pattern = /^\d*$/;
    let stringvalue = control.value.toString();

    if (!stringvalue.match(pattern))
        return { 'invalidNumber': true };

    return null;
}
