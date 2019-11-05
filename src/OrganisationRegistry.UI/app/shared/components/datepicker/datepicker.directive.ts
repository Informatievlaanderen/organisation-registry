import { Directive, ElementRef, Input, Renderer, AfterViewInit } from '@angular/core';
import { UUID } from 'angular2-uuid';

declare var vl: any; // global vlaanderen ui

@Directive({
  selector: '[ww-datepicker]'
})
export class DatepickerDirective implements AfterViewInit {
  private element: HTMLElement;
  private elementAttribute = 'data-datepicker-id';

  constructor(element: ElementRef) {
    this.element = element.nativeElement;

    this.element.setAttribute(this.elementAttribute, UUID.UUID());
  }

  ngAfterViewInit() {
    vl.datepicker.dress(this.element);
    // eval(`vl.datepicker.dress(document.querySelector(\"[{this.datepickerAttribute}=\'{this.element.getAttribute(this.datepickerAttribute)}\']\"));`);
  }
}
