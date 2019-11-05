import { AbstractControl, FormGroup } from '@angular/forms';

export function atLeastOne(...controls: AbstractControl[]) {
  return (group: FormGroup): { [ key: string ]: any } => {
    if (controls.every(c => {
      if (c.value) {
        return c.value.toString().trim() === '';
      } else {
        return true;
      }
    })) {
      return {
        atLeastOneRequired: true
      };
    }
  };
}
