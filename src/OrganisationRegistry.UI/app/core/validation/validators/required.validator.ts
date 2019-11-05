import { AbstractControl, FormGroup } from '@angular/forms';


export function required(control: AbstractControl): { [key: string]: boolean } {
  if (!control.value || typeof control.value === 'string' && !control.value.trim()) {
    return {
      required: true
    };
  }

  return null;
}
