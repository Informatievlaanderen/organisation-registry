import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  Output,
  OnInit,
  OnDestroy,
  ElementRef,
  Renderer,
  ViewChild
} from '@angular/core';

import { FormControl } from '@angular/forms';
import { UUID } from 'angular2-uuid';

import { SelectItem } from './../form-group-select/form-group-select.model';

declare var vl: any; // global vlaanderen ui

@Component({
  selector: 'ww-form-group-taglist',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: 'form-group-taglist.template.html'
})
export class FormGroupTaglist implements OnInit {
  @Input() control: FormControl;
  @Input() id: string;
  @Input() label: string;
  @Input() placeholder: string;
  @Input() name: string;
  @Input() focus: boolean;
  @Input() inline: boolean;

  private _init = false;

  private _values: Array<SelectItem>;
  public get values(): Array<SelectItem> {
    return this._values;
  }

  @Input('values')
  public set values(value: Array<SelectItem>) {
    if (this._values === value)
      return;

    this._values = value;

    setTimeout(function () {
      if (!this._init) {
        // if you try to do this twice, you're plain out of luck, sorry...
        this._init = false;

        vl.multiselect.dress(this.selectElement);

        this.inputElement = <HTMLInputElement>this.element.querySelector('input.input-field');
        this.buttonElement = <HTMLButtonElement>this.element.querySelector('button.js-select__input');

        this.setDisabled(this._disabled);
        const pills = this.element.querySelectorAll('a.pill__close');
        if(this.control.disabled)
          pills.forEach(pill => {
            pill.removeListener("click");
            pill.removeAttribute("href")
          });
      }
    }.bind(this), 0);
  }

  private elementAttribute = 'data-id'; // used in vlaanderen-ui.js, so don't change
  private element: HTMLElement;
  private selectElement: HTMLSelectElement;
  private inputElement: HTMLInputElement;
  private buttonElement: HTMLButtonElement;

  private generatedGuid: string;

  constructor(
    element: ElementRef,
    private renderer: Renderer
  ) {
    this.element = element.nativeElement;
  }

  isSelectedItem(control, value) {
    let values = this.control.value;
    return values.indexOf(value) !== -1;
  }

  ngOnInit() {
    let nodeList = <HTMLSelectElement[]><any>this.element.querySelectorAll('[data-multiselect]');
    this.selectElement = nodeList[0];

    this.generatedGuid = UUID.UUID();
    this.selectElement.setAttribute(this.elementAttribute, this.generatedGuid);

    this.control.registerOnDisabledChange(disabled => this.setDisabled(disabled));
  }

  onValueChanged(e) {
    if (!(e.srcElement || e.target))
      return;

    let options = e.target.options;
    let newValues = [];
    for (let i = 0; i < options.length; i++) {
      if (options[i].selected)
        newValues.push(this.parseId(options[i].value));
    }

    this.control.setValue(newValues);
  }

  private setDisabled(value: boolean) {
    if (value === undefined)
      return;

    if (value) {
      if (this.buttonElement)
        vl.select.setDisabledState(this.selectElement, 'disabled');

    } else {
      if (this.buttonElement) {
        vl.select.setDisabledState(this.selectElement, '');

        if (this.focus)
          this.renderer.invokeElementMethod(this.buttonElement, 'focus');
      }
    }
  }

  private parseId(value: string) {
    let guidRegex = /'(.*)'/;
    // 0: '1d6c67cd-6943-f558-a7fd-c2f1b288c234'

    let groups = guidRegex.exec(value);
    return (groups && groups.length === 2) ? groups[1] : '';
  }
}
