import { Input, Component, ChangeDetectionStrategy, AfterViewInit, ElementRef, Renderer, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormControl } from '@angular/forms';

import { SelectItem } from './form-group-select.model';

@Component({
  selector: 'ww-form-group-select',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-select.template.html',
  styleUrls: ['form-group-select.style.css']
})
export class FormGroupSelect implements AfterViewInit {
  @ViewChild('mySelect') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;
  @Input() values: Array<SelectItem>;
  @Input() alwaysValidate: boolean;

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
