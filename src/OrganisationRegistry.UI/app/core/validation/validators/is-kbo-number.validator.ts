import { AbstractControl, FormGroup } from '@angular/forms';

export function isKboNumber(control: AbstractControl): { [key: string]: boolean } {
  if (!control.value || typeof control.value === 'string' && !control.value.trim()) {
    return { 'invalidKboNumber': true };
  }

  let pattern = /^[01](([0-9]){3}\.?){2}([0-9]){3}$/;
  let stringvalue = control.value.toString();

  if (!stringvalue.match(pattern))
    return { 'invalidKboNumber': true };

  return null;
}

