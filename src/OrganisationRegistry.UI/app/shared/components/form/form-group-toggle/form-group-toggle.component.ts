import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AfterViewInit, ElementRef, Renderer, ViewChild } from '@angular/core';

@Component({
  selector: 'ww-form-group-toggle',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-toggle.template.html',
  styleUrls: ['form-group-toggle.style.css']
})
export class FormGroupToggle implements AfterViewInit {
  @ViewChild('myToggle') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;

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
}
