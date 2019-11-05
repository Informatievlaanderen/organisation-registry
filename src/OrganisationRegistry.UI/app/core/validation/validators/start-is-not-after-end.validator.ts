import { AbstractControl, FormGroup } from '@angular/forms';

export function startIsNotAfterEnd(startDate: AbstractControl, endDate: AbstractControl) {
  return (group: FormGroup): { [key: string]: any } => {
    // console.log('startDate', startDate);
    // console.log('endDate', endDate);

    if (startDate.value && endDate.value && startDate.value > endDate.value) {
      let errors = {
        startDateIsAfterEndDate: true
      };

      populateErrors(startDate);
      populateErrors(endDate);

      return errors;
    } else {
      if (startDate.errors)
        clearErrors(startDate);

      if (endDate.errors)
        clearErrors(endDate);

      return null;
    }
  };
}

function populateErrors(control: AbstractControl) {
  let errors = control.errors;
  if (!errors)
    errors = {};

  if (control.hasError('startDateIsAfterEndDate'))
    return;

  errors['startDateIsAfterEndDate'] = true;
  control.setErrors(errors);
  control.markAsTouched();
}

function clearErrors(control: AbstractControl) {
  if (control.hasError('startDateIsAfterEndDate')) {
    let errors = control.errors;
    delete errors['startDateIsAfterEndDate'];

    var hasAnyErrors = false; for (var key in errors) { hasAnyErrors = true; break; }
    control.setErrors(hasAnyErrors ? errors : null, { emitEvent: false });
  }
}
