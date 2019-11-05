import { Directive, ElementRef, Input, Renderer, OnInit } from '@angular/core';
import { UUID } from 'angular2-uuid';

declare var vl: any; // global vlaanderen ui

@Directive({
  selector: '[ww-popup]'
})
export class PopupDirective implements OnInit {
  private element: HTMLElement;
  private elementAttribute = 'data-popup-id';

  constructor(element: ElementRef) {
    this.element = element.nativeElement;

    this.element.setAttribute(this.elementAttribute, UUID.UUID());
  }

  ngOnInit() {
    vl.popup.dress(this.element);
    // eval(`vl.popup.dress(document.querySelector(\"[{this.elementAttribute}=\'{this.element.getAttribute(this.elementAttribute)}\']\"));`);
  }
}
