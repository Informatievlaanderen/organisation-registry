import { Input, Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { FormControl, ValidatorFn, Validators } from '@angular/forms';

import { date, startIsNotAfterEnd, required } from 'core/validation';

@Component({
  selector: 'ww-form-group-datepicker-range',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-datepicker-range.template.html',
  styleUrls: ['form-group-datepicker-range.style.css']
})
export class FormGroupDatepickerRange implements OnInit {
  @Input() fromControl: FormControl;
  @Input() fromId: string;
  @Input() fromLabel: string;
  @Input() fromPlaceholder: string;
  @Input() fromName: string;
  @Input() fromFocus: boolean;
  @Input() fromInline: boolean;
  @Input() fromMinDate: string = '1800-01-01';
  @Input() fromMaxDate: string = '2100-12-31';

  @Input() toControl: FormControl;
  @Input() toId: string;
  @Input() toLabel: string;
  @Input() toPlaceholder: string;
  @Input() toName: string;
  @Input() toFocus: boolean;
  @Input() toInline: boolean;
  @Input() toMinDate: string = '1800-01-01';
  @Input() toMaxDate: string = '2100-12-31';

  ngOnInit() {
    this.fromControl.setValidators(Validators.compose([this.fromControl.validator, ...this.getValidators()]));

    this.toControl.setValidators(Validators.compose([this.toControl.validator, ...this.getValidators()]));
  }

  private getValidators() {
    let startIsNotAfterEndValidator = startIsNotAfterEnd(this.fromControl, this.toControl);

    let validators = [
      date,
      startIsNotAfterEndValidator
    ];

    return validators;
  }
}
