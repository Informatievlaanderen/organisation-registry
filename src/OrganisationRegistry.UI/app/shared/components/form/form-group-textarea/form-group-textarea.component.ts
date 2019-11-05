import { Input, Component, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AfterViewInit, ElementRef, Renderer, ViewChild } from '@angular/core';

@Component({
  selector: 'ww-form-group-textarea',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-textarea.template.html'
})
export class FormGroupTextArea implements AfterViewInit {
  @ViewChild('myTextArea') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() rows: number;

  constructor(
    private renderer: Renderer,
    private changeDetection: ChangeDetectorRef
  ) { }

  ngAfterViewInit() {

    this.control.registerOnDisabledChange(disabled => this.changeDetection.markForCheck());

    if (this.input && this.input.nativeElement && this.focus)
      this.renderer.invokeElementMethod(this.input.nativeElement, 'focus');
  }
}
