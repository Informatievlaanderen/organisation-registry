import {AbstractControl} from "@angular/forms";

export function isOptionalPdf(control: AbstractControl): { [key: string]: boolean } {
  if (control == null || control.value == null || control.value.length === 0)
    return null;

  const pattern = /^.+\.[pP][dD][fF]$/
  const stringValue = control.value.toString();

  if (!stringValue.match(pattern))
    return {'invalidPdf': true};

  return null;
}
