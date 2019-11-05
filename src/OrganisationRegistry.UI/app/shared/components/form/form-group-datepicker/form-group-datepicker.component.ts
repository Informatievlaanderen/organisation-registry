import { Input, Component, ChangeDetectionStrategy, ChangeDetectorRef, DoCheck } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AfterViewInit, ElementRef, Renderer, ViewChild } from '@angular/core';

import * as moment from 'moment/moment';

@Component({
  selector: 'ww-form-group-datepicker',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-datepicker.template.html',
  styleUrls: ['form-group-datepicker.style.css']
})
export class FormGroupDatepicker implements AfterViewInit, DoCheck {
  @ViewChild('myDate') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;

  @Input() validFromLabel: string;
  @Input() validToLabel: string;

  @Input() minDate: string = '1800-01-01';
  @Input() maxDate: string = '2100-12-31';

  private dateFormat = 'YYYY-MM-DD';
  private currentValue: string = '';
  private hasStartDateIsAfterEndDateError: boolean = false;

  constructor(
    private renderer: Renderer,
    private changeDetection: ChangeDetectorRef
  ) { }

  ngAfterViewInit() {
    this.control.registerOnDisabledChange(disabled => this.changeDetection.markForCheck());

    if (this.input && this.input.nativeElement && this.focus)
      this.renderer.invokeElementMethod(this.input.nativeElement, 'focus');
  }

  ngDoCheck() {
    let hasStartDateIsAfterEndDateError = this.control.hasError('startDateIsAfterEndDate');

    if (this.hasStartDateIsAfterEndDateError !== hasStartDateIsAfterEndDateError) {
      this.hasStartDateIsAfterEndDateError = hasStartDateIsAfterEndDateError;
      this.changeDetection.markForCheck();
    }
  }

  public onValueChanged(e) {
    if (!(e.srcElement || e.target))
      return;

    let newValue = e.target.value;

    if (newValue === this.currentValue)
      return;

    if (this.dateValidator(newValue)) {
      this.currentValue = newValue;
      this.control.setValue(newValue);
    }
  }

  private dateValidator(value: string): boolean {
    if (value == null || value.toString().length === 0)
      return true;

    let pattern = /^\d{4}\-\d{2}\-\d{2}$/; // ####-##-##
    let stringvalue = value.toString();

    if (!stringvalue.match(pattern))
      return false;

    return true;
  }
}
