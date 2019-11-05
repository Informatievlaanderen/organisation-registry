import { Input, Component, ChangeDetectionStrategy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AfterViewInit, ElementRef, Renderer, ViewChild } from '@angular/core';
import { RadioItem } from './form-group-radio.model';

@Component({
  selector: 'ww-form-group-radiolist',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-radio.template.html',
  styleUrls: ['form-group-radio.style.css']
})
export class FormGroupRadio implements AfterViewInit {
  // @ViewChild('myRadio') input: ElementRef;

  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() name: string;
  @Input() inline: boolean;
  @Input() values: Array<RadioItem>;

  constructor(
    private renderer: Renderer
  ) { }

  ngAfterViewInit() {
    // if (this.input && this.input.nativeElement && this.focus)
    //   this.renderer.invokeElementMethod(this.input.nativeElement, 'focus');
  }
}
