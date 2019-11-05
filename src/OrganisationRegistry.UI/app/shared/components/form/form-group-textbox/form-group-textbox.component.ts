import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, DoCheck, ChangeDetectorRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AfterViewInit, ElementRef, Renderer, ViewChild } from '@angular/core';

@Component({
  selector: 'ww-form-group-textbox',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-textbox.template.html'
})
export class FormGroupTextbox implements AfterViewInit, DoCheck {
  @ViewChild('myInput') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;

  @Output() valueChanged = new EventEmitter<any>();

  private hasRequiredError: boolean = false;
  private hasInvalidNumberError: boolean = false;

  constructor(
    private renderer: Renderer,
    private changeDetection: ChangeDetectorRef
  ) { }

  ngAfterViewInit() {
    this.control.registerOnDisabledChange(disabled => {
      this.changeDetection.markForCheck();
    });

    if (this.input && this.input.nativeElement && this.focus)
      this.renderer.invokeElementMethod(this.input.nativeElement, 'focus');
  }

  ngDoCheck() {
    let hasRequiredError = this.control.hasError('required');
    if (this.hasRequiredError !== hasRequiredError) {
      this.hasRequiredError = hasRequiredError;
      this.changeDetection.markForCheck();
    }

    let hasInvalidNumberError = this.control.hasError('invalidNumber');
    if (this.hasInvalidNumberError !== hasInvalidNumberError) {
      this.hasInvalidNumberError = hasInvalidNumberError;
      this.changeDetection.markForCheck();
    }

    if (this.control.touched)
      this.changeDetection.markForCheck();
  }

  onValueChanged(data) {
    this.valueChanged.next(data);
  }
}
